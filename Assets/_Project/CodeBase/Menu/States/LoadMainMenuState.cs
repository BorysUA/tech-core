using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Menu.States
{
  public class LoadMainMenuState : IEnterState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IWindowsService _windowsService;

    private readonly List<IMenuInitAsync> _onLoadInitAsync;

    public LoadMainMenuState(GameStateMachine gameStateMachine, IWindowsService windowsService,
      List<IMenuInitAsync> onLoadInitAsync)
    {
      _gameStateMachine = gameStateMachine;
      _windowsService = windowsService;
      _onLoadInitAsync = onLoadInitAsync;
    }

    public void Enter() =>
      InternalEnterAsync().Forget();

    private async UniTaskVoid InternalEnterAsync()
    {
      await InitializeServices();

      await _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
      _gameStateMachine.Enter<MainMenuState>();
    }

    private async UniTask InitializeServices()
    {
      List<UniTask> initializationTasks = new List<UniTask>();

      foreach (IMenuInitAsync initializable in _onLoadInitAsync)
        initializationTasks.Add(initializable.InitializeAsync());

      await UniTask.WhenAll(initializationTasks);
    }
  }
}