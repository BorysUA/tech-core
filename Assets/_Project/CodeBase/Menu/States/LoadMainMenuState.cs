using System.Collections.Generic;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.UI.Services;
using Zenject;

namespace _Project.CodeBase.Menu.States
{
  public class LoadMainMenuState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IWindowsService _windowsService;

    public LoadMainMenuState(GameStateMachine gameStateMachine, IWindowsService windowsService)
    {
      _gameStateMachine = gameStateMachine;
      _windowsService = windowsService;
    }

    public void Enter()
    {
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
      _gameStateMachine.Enter<MainMenuState>();
    }

    public void Exit()
    {
    }
  }
}