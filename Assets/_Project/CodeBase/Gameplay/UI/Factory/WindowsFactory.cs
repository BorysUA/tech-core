using System;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.Exceptions;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.AssetsPipeline;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public class WindowsFactory : IWindowsFactory
  {
    private const int MaxRetryCount = 2;

    private readonly IInstantiator _instantiator;
    private readonly AddressMap _addressMap;
    private readonly IAssetProvider _assetProvider;
    private readonly WindowsRepository _windowsRepository;
    private readonly WindowsCanvas _windowsCanvas;
    private readonly ILogService _logService;

    private readonly Dictionary<(Type, Type), UniTaskCompletionSource<IWindowCreationResult<BaseWindowViewModel>>>
      _cachedTasks = new();

    public WindowsFactory(AddressMap addressMap, IAssetProvider assetProvider, WindowsCanvas windowsCanvas,
      WindowsRepository windowsRepository, ILogService logService, IInstantiator instantiator)
    {
      _addressMap = addressMap;
      _assetProvider = assetProvider;
      _windowsCanvas = windowsCanvas;
      _windowsRepository = windowsRepository;
      _logService = logService;
      _instantiator = instantiator;
    }

    public async UniTask<WindowCreationResult<TViewModel>> CreateWindow<TWindow, TViewModel>(CancellationToken token,
      bool useCache = true)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      (Type, Type) key = (typeof(TWindow), typeof(TViewModel));

      if (_cachedTasks.TryGetValue(key,
            out UniTaskCompletionSource<IWindowCreationResult<BaseWindowViewModel>> cachedTcs))
        return (WindowCreationResult<TViewModel>)await cachedTcs.Task;

      UniTaskCompletionSource<IWindowCreationResult<BaseWindowViewModel>> tcs = new();
      _cachedTasks.Add(key, tcs);

      try
      {
        WindowCreationResult<TViewModel> result = await CreateWindowInternal<TWindow, TViewModel>(token, useCache);
        tcs.TrySetResult(result);
        return result;
      }
      catch (Exception ex)
      {
        tcs.TrySetException(ex);
        throw;
      }
      finally
      {
        _cachedTasks.Remove(key);
      }
    }

    private async UniTask<WindowCreationResult<TViewModel>> CreateWindowInternal<TWindow, TViewModel>(
      CancellationToken token,
      bool useCache = true)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      if (useCache && _windowsRepository.TryGetValue<TViewModel>(out BaseWindowViewModel cachedWindowViewModel))
        return new WindowCreationResult<TViewModel>(instance: cachedWindowViewModel as TViewModel, fromCache: true);

      TViewModel viewModel = _instantiator.Instantiate<TViewModel>();

      string address = _addressMap.GetAddress<TWindow>();
      GameObject windowPrefab = await _assetProvider.LoadAssetAsync<GameObject>(address, token);
      TWindow window = _instantiator.InstantiatePrefabForComponent<TWindow>(windowPrefab, _windowsCanvas.Root);

      window.Setup(viewModel);

      if (useCache)
        _windowsRepository.Register<TViewModel>(viewModel);

      return new WindowCreationResult<TViewModel>(instance: viewModel, fromCache: false);
    }
  }
}