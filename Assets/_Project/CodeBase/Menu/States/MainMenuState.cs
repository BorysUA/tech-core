using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine.States;
using _Project.CodeBase.Menu.Signals;
using Zenject;

namespace _Project.CodeBase.Menu.States
{
  public class MainMenuState : IEnterState, IExitState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IDataTransferService _dataTransferService;
    private readonly SignalBus _signalBus;

    public MainMenuState(GameStateMachine gameStateMachine, SignalBus signalBus,
      IDataTransferService dataTransferService)
    {
      _gameStateMachine = gameStateMachine;
      _signalBus = signalBus;
      _dataTransferService = dataTransferService;
    }

    public void Enter()
    {
      _signalBus.Subscribe<GameplaySceneLoadRequested>(OnLoadGameplayState);
    }

    public void Exit()
    {
      _signalBus.Unsubscribe<GameplaySceneLoadRequested>(OnLoadGameplayState);
    }

    private void OnLoadGameplayState(GameplaySceneLoadRequested args)
    {
      _dataTransferService.SetData(args.GameplaySettings);
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.Gameplay);
    }
  }
}