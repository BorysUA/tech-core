using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public abstract class BaseStateMachine
  {
    protected Dictionary<Type, IExitState> States { get; } = new();
    private IExitState _currentState;

    public void Enter<TState>() where TState : class, IState
    {
      IState state = GetState<TState>();
      ChangeState(state);
      state.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
    {
      IPayloadState<TPayload> gameState = GetState<TState>();
      ChangeState(gameState);
      gameState.Enter(payload);
    }

    public void RegisterState<TState>(TState gameState) where TState : IExitState
    {
      States.Add(typeof(TState), gameState);

      if (gameState is IInitializableState state) 
        state.Initialize();
    }

    public void ExitCurrentState()
    {
      _currentState?.Exit();
    }

    private void ChangeState(IExitState gameState)
    {
      _currentState?.Exit();
      _currentState = gameState;
    }

    private TState GetState<TState>() where TState : class, IExitState =>
      States[typeof(TState)] as TState;
  }
}