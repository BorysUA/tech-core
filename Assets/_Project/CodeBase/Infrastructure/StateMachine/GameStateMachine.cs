using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public class GameStateMachine : BaseStateMachine
  {
    private readonly List<Type> _sceneStateKeys = new();

    public void RegisterSceneState<TState>(TState gameState) where TState : IExitState
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
  }
}