using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine.States;
using _Project.CodeBase.Menu.States;
using Zenject;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class GameplayState : IEnterState, IExitState
  {
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly GameplayPhaseFlow _gameplayPhaseFlow;
    private readonly GameStateMachine _gameStateMachine;
    private readonly SignalBus _signalBus;

    public GameplayState(GameplayStateMachine gameplayStateMachine, GameplayPhaseFlow gameplayPhaseFlow,
      GameStateMachine gameStateMachine, SignalBus signalBus)
    {
      _gameplayStateMachine = gameplayStateMachine;
      _gameplayPhaseFlow = gameplayPhaseFlow;
      _gameStateMachine = gameStateMachine;
      _signalBus = signalBus;
    }

    public void Enter()
    {
      _gameplayPhaseFlow.SetPhase(GameplayPhase.Started);
      _gameplayStateMachine.Enter<DefaultGameplayState>();

      _signalBus.Subscribe<ExitToMenuRequested>(LoadMenuState);
    }

    public void Exit()
    {
      _gameplayPhaseFlow.SetPhase(GameplayPhase.Ended);
      _gameplayStateMachine.ExitCurrentState();
      _signalBus.Unsubscribe<ExitToMenuRequested>(LoadMenuState);
    }

    private void LoadMenuState() =>
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.MainMenu);
  }
}