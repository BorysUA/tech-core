using System;
using System.Threading;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Common;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.UI.Services
{
  public class WindowsService : IWindowsService
  {
    private readonly IWindowsFactory _windowsFactory;
    private readonly Subject<BaseWindowViewModel> _windowOpened = new();
    private readonly ILogService _logService;

    private BaseWindowViewModel _currentWindow;
    private DisposableBag _disposable;

    public Observable<BaseWindowViewModel> WindowOpened => _windowOpened;

    public WindowsService(IWindowsFactory windowsFactory, ILogService logService)
    {
      _windowsFactory = windowsFactory;
      _logService = logService;
    }

    public async UniTask OpenWindow<TWindow, TViewModel>(bool loadFromCache = true, CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      await OpenInternal<TWindow, TViewModel>(_ => { }, token, loadFromCache);
    }

    public async UniTask OpenWindow<TWindow, TViewModel, TData>(TData data, bool loadFromCache = true,
      CancellationToken token = default)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel, IParameterizedWindow<TData>
    {
      await OpenInternal<TWindow, TViewModel>(viewModel => viewModel.Initialize(data), token, loadFromCache);
    }

    private async UniTask OpenInternal<TWindow, TViewModel>(Action<TViewModel> configure, CancellationToken token,
      bool loadFromCache)
      where TWindow : IWindow
      where TViewModel : BaseWindowViewModel
    {
      _currentWindow?.Close();

      try
      {
        TViewModel viewModel = await _windowsFactory.CreateWindow<TWindow, TViewModel>(token, loadFromCache);

        configure(viewModel);

        _currentWindow = viewModel;
        viewModel.WindowClosed
          .Subscribe(_ => CleanupWindow())
          .AddTo(ref _disposable);

        _windowOpened.OnNext(_currentWindow);
        _currentWindow.Open();
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