using System;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.Exceptions;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI;
using _Project.CodeBase.UI.Common;
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

    private readonly Dictionary<(Type, Type), UniTaskCompletionSource<BaseWindowViewModel>> _cachedTasks = new();

    public WindowsFactory(IInstantiator instantiator, AddressMap addressMap, IAssetProvider assetProvider,
      WindowsCanvas windowsCanvas, WindowsRepository windowsRepository, ILogService logService)
    {
      _instantiator = instantiator;
      _addressMap = addressMap;
      _assetProvider = assetProvider;
      _windowsCanvas = windowsCanvas;
      _windowsRepository = windowsRepository;
      _logService = logService;
    }

    public async UniTask<TViewModel> CreateWindow<TWindow, TViewModel>(CancellationToken token, bool useCache = true)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      (Type, Type) key = (typeof(TWindow), typeof(TViewModel));

      if (_cachedTasks.TryGetValue(key, out UniTaskCompletionSource<BaseWindowViewModel> cachedTcs))
        return (TViewModel)await cachedTcs.Task;

      UniTaskCompletionSource<BaseWindowViewModel> tcs = new UniTaskCompletionSource<BaseWindowViewModel>();
      _cachedTasks.Add(key, tcs);

      try
      {
        TViewModel viewModel = await CreateWindowInternal<TWindow, TViewModel>(token, useCache);
        tcs.TrySetResult(viewModel);
        return viewModel;
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

    private async UniTask<TViewModel> CreateWindowInternal<TWindow, TViewModel>(CancellationToken token,
      bool useCache = true)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      if (useCache && _windowsRepository.TryGetValue<TViewModel>(out BaseWindowViewModel cachedWindowViewModel))
        return cachedWindowViewModel as TViewModel;

      TViewModel viewModel = _instantiator.Instantiate<TViewModel>();

      if (viewModel is IAsyncInitializable initializable)
        await initializable.InitializeAsync();

      string address = _addressMap.GetAddress<TWindow>();
      GameObject windowPrefab = await _assetProvider.LoadAssetAsync<GameObject>(address, token);
      TWindow window = _instantiator.InstantiatePrefabForComponent<TWindow>(windowPrefab, _windowsCanvas.Root);

      window.Setup(viewModel);

      if (useCache)
        _windowsRepository.Register<TViewModel>(viewModel);

      return viewModel;
    }
  }
}