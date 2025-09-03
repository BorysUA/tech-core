using System;
using System.Threading;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.UI.Services
{
  public class WindowsService : IWindowsService, IGameplayInit
  {
    private readonly IWindowsFactory _windowsFactory;
    private readonly Subject<BaseWindowViewModel> _windowOpened = new();
    private readonly ReactiveProperty<int> _openWindowCount = new(0);
    private readonly ILogService _logService;

    private BaseWindowViewModel _currentWindow;
    private DisposableBag _disposable;

    public ReadOnlyReactiveProperty<bool> AnyWindowOpen { get; private set; }
    public Observable<BaseWindowViewModel> WindowOpened => _windowOpened;

    public InitPhase InitPhase => InitPhase.Preparation;

    public WindowsService(IWindowsFactory windowsFactory, ILogService logService)
    {
      _windowsFactory = windowsFactory;
      _logService = logService;
    }

    public void Initialize()
    {
      AnyWindowOpen = _openWindowCount.Select(x => x > 0).ToReadOnlyReactiveProperty();
    }

    public async UniTask OpenWindow<TWindow, TViewModel>(bool loadFromCache = true, CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      await OpenInternal<TWindow, TViewModel>((viewModel, fromCache) =>
      {
        if (fromCache)
          viewModel.Reset();

        viewModel.Initialize();
      }, token, loadFromCache);
    }

    public async UniTask OpenWindow<TWindow, TViewModel, TParam>(TParam param, bool loadFromCache = true,
      CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel, IParameterizedWindow<TParam>
    {
      await OpenInternal<TWindow, TViewModel>((viewModel, fromCache) =>
        {
          if (fromCache && viewModel.Matches(param))
            return;

          if (fromCache)
            viewModel.Reset();

          viewModel.Initialize(param);
        }
        , token, loadFromCache);
    }

    private async UniTask OpenInternal<TWindow, TViewModel>(Action<TViewModel, bool> configure, CancellationToken token,
      bool loadFromCache)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      _currentWindow?.Close();

      try
      {
        WindowCreationResult<TViewModel> result =
          await _windowsFactory.CreateWindow<TWindow, TViewModel>(token, loadFromCache);

        (TViewModel viewModel, bool fromCache) = result;

        configure(viewModel, fromCache);

        _currentWindow = viewModel;
        viewModel.WindowClosed
          .Subscribe(_ =>
          {
            _openWindowCount.Value = Math.Max(0, _openWindowCount.Value - 1);
            CleanupWindow();
          })
          .AddTo(ref _disposable);

        _windowOpened.OnNext(_currentWindow);
        _currentWindow.Open();
        _openWindowCount.Value++;
      }
      catch (OperationCanceledException)
      {
        _logService.LogInfo(GetType(),
          $"Opening '{typeof(TWindow).Name}' was canceled by token.");
      }
      catch (Exception exception)
      {
        _logService.LogError(GetType(),
          $"Error opening window '{typeof(TWindow).Name}'", exception);
      }
    }

    private void CleanupWindow()
    {
      _currentWindow = null;
      _disposable.Clear();
    }
  }
}