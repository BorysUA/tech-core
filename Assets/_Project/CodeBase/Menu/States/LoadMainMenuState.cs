using System.Collections.Generic;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Menu.UI.Window;
using _Project.CodeBase.UI.Services;

namespace _Project.CodeBase.Menu.States
{
  public class LoadMainMenuState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IWindowsService _windowsService;
    private readonly List<IOnLoadInitializableAsync> _onLoadInitializables;

    public LoadMainMenuState(GameStateMachine gameStateMachine, IWindowsService windowsService,
      List<IOnLoadInitializableAsync> onLoadInitializables)
    {
      _gameStateMachine = gameStateMachine;
      _windowsService = windowsService;
      _onLoadInitializables = onLoadInitializables;
    }

    public void Enter()
    {
      InitializeServices();
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
      _gameStateMachine.Enter<MainMenuState>();
    }

    public void Exit()
    {
    }

    private async void InitializeServices()
    {
      foreach (IOnLoadInitializableAsync initializable in _onLoadInitializables)
        await initializable.InitializeAsync();
    }
  }
}