using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine.States;
using _Project.CodeBase.Menu.Signals;
using Zenject;

namespace _Project.CodeBase.Menu.States
{
  public class MainMenuState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IDataTransferService _dataTransferService;
    private readonly SignalBus _signalBus;
    private readonly IAssetProvider _assetProvider;

    public MainMenuState(GameStateMachine gameStateMachine, SignalBus signalBus,
      IDataTransferService dataTransferService, IAssetProvider assetProvider)
    {
      _gameStateMachine = gameStateMachine;
      _signalBus = signalBus;
      _dataTransferService = dataTransferService;
      _assetProvider = assetProvider;
    }

    public void Enter()
    {
      _signalBus.Subscribe<LoadGameplaySignal>(OnLoadGameplayState);
    }

    private void OnLoadGameplayState(LoadGameplaySignal args)
    {
      _dataTransferService.SetData(args.GameplaySettings);
      _assetProvider.CleanUp();
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.Gameplay);
    }

    public void Exit()
    {
      _signalBus.Unsubscribe<LoadGameplaySignal>(OnLoadGameplayState);
    }
  }
}