using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class GameplayState : IState
  {
    private readonly MeteoriteSpawner _meteoriteSpawner;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly ISessionTimer _sessionTimer;

    public GameplayState(MeteoriteSpawner meteoriteSpawner, GameplayStateMachine gameplayStateMachine,
      ISessionTimer sessionTimer)
    {
      _meteoriteSpawner = meteoriteSpawner;
      _gameplayStateMachine = gameplayStateMachine;
      _sessionTimer = sessionTimer;
    }

    public void Enter()
    {
      _sessionTimer.Start();
      _meteoriteSpawner.Start();
      _gameplayStateMachine.Enter<DefaultGameplayState>();
    }

    public void Exit()
    {
      _sessionTimer.Pause();
      _gameplayStateMachine.ExitCurrentState();
      _meteoriteSpawner.Stop();
    }
  }
}