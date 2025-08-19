using System;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Infrastructure.Exceptions;
using _Project.CodeBase.Infrastructure.Guards;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.Infrastructure.Services.AssetsPipeline
{
  public class AssetProvider : IAssetProvider, IBootstrapInitAsync
  {
    private readonly ILogService _logService;
    private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
    private readonly Dictionary<string, AsyncOperationHandle> _cachedAssets = new();
    private readonly UniTaskCompletionSource _whenReadyTcs = new();

    public UniTask WhenReady => _whenReadyTcs.Task.WithCycleGuard(this);

    public AssetProvider(ILogService logService)
    {
      _logService = logService;
    }

    public async UniTask InitializeAsync()
    {
      await Addressables.InitializeAsync();
      _whenReadyTcs.TrySetResult();
    }

    public async UniTask<T> LoadAssetAsyncFromResources<T>(string path) where T : Object =>
      await Resources.LoadAsync<T>(path).ToUniTask() as T;

    public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken token, int retryCount = 1)
      where T : class =>
      await InternalLoadAssetAsync(MakeKey(address, typeof(T)), () => Addressables.LoadAssetAsync<T>(address), token,
        retryCount);

    public async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, CancellationToken token,
      int retryCount = 1)
      where T : class =>
      await InternalLoadAssetAsync(MakeKey($"{assetReference.RuntimeKey}", typeof(T)),
        () => Addressables.LoadAssetAsync<T>(assetReference), token, retryCount);

    public async UniTask<IList<T>> LoadAssetsAsync<T>(string address, CancellationToken token = default,
      int retryCount = 1)
      where T : class
      => await InternalLoadAssetAsync(MakeKey(address, typeof(T)), () => Addressables.LoadAssetsAsync<T>(address),
        token, retryCount);

    public async UniTask PreloadAssetsAsync(string label)
    {
      string key = MakeKey(label, typeof(IList<object>));
      AsyncOperationHandle<IList<object>> handle = Addressables.LoadAssetsAsync<object>(label);
      AddHandleToCache(key, handle);

      await handle.Task;

      if (handle.Status == AsyncOperationStatus.Succeeded)
        _cachedAssets.TryAdd(key, handle);
      else
        _logService.LogError(GetType(), $"Failed to load asset with specified label '{label}'");
    }

    private async UniTask<T> InternalLoadAssetAsync<T>(string key, Func<AsyncOperationHandle<T>> loader,
      CancellationToken token, int retryCount = 1) where T : class
    {
      if (_cachedAssets.TryGetValue(key, out AsyncOperationHandle cachedHandle))
        return cachedHandle.Result as T;

      AsyncOperationHandle<T> handle = loader.Invoke();

      try
      {
        return await RunWithCache<T>(key, handle, token);
      }
      catch (AssetLoadException)
      {
        if (retryCount > 0)
          return await InternalLoadAssetAsync(key, loader, token, retryCount - 1);

        throw new AssetLoadException($"Failed to load asset '{key}' after {retryCount + 1} attempts");
      }
    }

    private async UniTask<T> RunWithCache<T>(string key, AsyncOperationHandle handle, CancellationToken token = default)
      where T : class
    {
      try
      {
        AddHandleToCache(key, handle);

        await handle
          .Task
          .AsUniTask()
          .AttachExternalCancellation(token);

        if (handle.Status != AsyncOperationStatus.Succeeded)
          throw new AssetLoadException($"Failed to load asset with specified key '{key}'");
      }
      finally
      {
        if (handle.Status != AsyncOperationStatus.Succeeded)
          handle.Release();
      }

      _cachedAssets.TryAdd(key, handle);
      return (T)handle.Result;
    }

    private string MakeKey(string baseKey, Type resultType)
      => $"{baseKey}:{resultType.FullName}";

    private void AddHandleToCache(string key, AsyncOperationHandle handle)
    {
      if (!_handles.TryAdd(key, new List<AsyncOperationHandle> { handle }))
        _handles[key].Add(handle);
    }

    public void CleanUp()
    {
      foreach (var entryCache in _handles)
      foreach (AsyncOperationHandle handle in entryCache.Value)
        handle.Release();

      _handles.Clear();
      _cachedAssets.Clear();
    }
  }
}