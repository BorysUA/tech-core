using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public class GameStateMachine : BaseStateMachine
  {
    private readonly List<Type> _sceneStateKeys = new();
    private readonly ILogService _logService;

    public GameStateMachine(ILogService logService)
    {
      _logService = logService;
    }

    public void RegisterSceneState<TState>(TState gameState) where TState : IState
    {
      RegisterState(gameState);
      _sceneStateKeys.Add(typeof(TState));
    }

    public void ClearSceneStates()
    {
      foreach (Type sceneStateKey in _sceneStateKeys)
        States.Remove(sceneStateKey);

      _sceneStateKeys.Clear();
    }

    protected override void OnStateEnter(IState state) =>
      _logService.LogInfo(GetType(), $"Enter {state.GetType().Name}");

    protected override void OnStateExit(IState state) =>
      _logService.LogInfo(GetType(), $"Exit  {state.GetType().Name}");
  }
}