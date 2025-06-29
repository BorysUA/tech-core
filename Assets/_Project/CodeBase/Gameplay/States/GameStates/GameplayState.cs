using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Timers;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class GameplayState : IState
  {
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly ISessionTimer _sessionTimer;

    public GameplayState(GameplayStateMachine gameplayStateMachine,
      ISessionTimer sessionTimer)
    {
      _gameplayStateMachine = gameplayStateMachine;
      _sessionTimer = sessionTimer;
    }

    public void Enter()
    {
      _sessionTimer.Start();
      _gameplayStateMachine.Enter<DefaultGameplayState>();
    }

    public void Exit()
    {
      _sessionTimer.Pause();
      _gameplayStateMachine.ExitCurrentState();
    }
  }
}