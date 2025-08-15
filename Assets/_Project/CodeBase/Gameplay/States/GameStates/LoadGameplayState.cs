using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Gameplay.States.GameplayStates.Placement;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class LoadGameplayState : IEnterState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IGameplayUiFactory _gameplayUiFactory;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly GameStatesFactory _gameStatesFactory;

    private readonly List<IGameplayInit> _onLoadInit;
    private readonly List<IGameplayInitAsync> _onLoadInitAsync;

    public LoadGameplayState(IGameplayUiFactory gameplayUiFactory,
      GameStateMachine gameStateMachine, GameplayStateMachine gameplayStateMachine, GameStatesFactory gameStatesFactory,
      List<IGameplayInit> onLoadInit, List<IGameplayInitAsync> onLoadInitAsync)
    {
      _gameplayUiFactory = gameplayUiFactory;
      _gameStateMachine = gameStateMachine;
      _gameplayStateMachine = gameplayStateMachine;
      _gameStatesFactory = gameStatesFactory;
      _onLoadInit = onLoadInit;
      _onLoadInitAsync = onLoadInitAsync;
    }

    public void Enter() =>
      InternalEnterAsync().Forget();

    private async UniTaskVoid InternalEnterAsync()
    {
      await InitializeServices();
      await InitializeUI();
      RegisterGameplayStates();

      _gameStateMachine.Enter<GameplayState>();
    }

    private UniTask InitializeUI() =>
      _gameplayUiFactory.CreateHud();

    private void RegisterGameplayStates()
    {
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<PlaceBuildingState>());
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<DefaultGameplayState>());
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<PlaceConstructionPlotState>());
    }

    private async UniTask InitializeServices()
    {
      List<UniTask> initializationTasks = new List<UniTask>();

      foreach (IGameplayInitAsync initializable in _onLoadInitAsync)
        initializationTasks.Add(initializable.InitializeAsync());

      await UniTask.WhenAll(initializationTasks);

      foreach (IGameplayInit initializable in _onLoadInit)
        initializable.Initialize();
    }
  }
}