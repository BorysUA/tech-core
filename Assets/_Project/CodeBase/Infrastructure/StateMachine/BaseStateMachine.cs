using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public abstract class BaseStateMachine
  {
    protected Dictionary<Type, IState> States { get; } = new();
    private IState _currentState;

    public void Enter<TState>() where TState : class, IState
    {
      IState state = GetState<TState>();
      ChangeState(state);

      if (state is IEnterState enterState)
        enterState.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
    {
      IPayloadState<TPayload> gameState = GetState<TState>();
      ChangeState(gameState);
      gameState.Enter(payload);
    }

    public void RegisterState<TState>(TState gameState) where TState : IState
    {
      States.Add(typeof(TState), gameState);

      if (gameState is IInitializableState state)
        state.Initialize();
    }

    public void ExitCurrentState()
    {
      if (_currentState is IExitState exitState)
        exitState.Exit();
    }

    protected virtual void OnStateEnter(IState state)
    {
    }

    protected virtual void OnStateExit(IState state)
    {
    }

    private void ChangeState(IState nextState)
    {
      if (_currentState != null)
      {
        if (_currentState is IExitState exitState)
          exitState.Exit();

        OnStateExit(_currentState);
      }

      _currentState = nextState;
      OnStateEnter(_currentState);
    }

    private TState GetState<TState>() where TState : class, IState =>
      States[typeof(TState)] as TState;
  }
}