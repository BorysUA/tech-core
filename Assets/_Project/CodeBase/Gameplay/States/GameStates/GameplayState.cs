using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class GameplayState : IEnterState, IExitState
  {
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly GameplayPhaseFlow _gameplayPhaseFlow;

    public GameplayState(GameplayStateMachine gameplayStateMachine, GameplayPhaseFlow gameplayPhaseFlow)
    {
      _gameplayStateMachine = gameplayStateMachine;
      _gameplayPhaseFlow = gameplayPhaseFlow;
    }

    public void Enter()
    {
      _gameplayPhaseFlow.SetPhase(GameplayPhase.Started);
      _gameplayStateMachine.Enter<DefaultGameplayState>();
    }

    public void Exit()
    {
      _gameplayStateMachine.ExitCurrentState();
      _gameplayPhaseFlow.SetPhase(GameplayPhase.Ended);
    }
  }
}