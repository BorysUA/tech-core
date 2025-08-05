using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Gameplay.States.GameplayStates.Placement;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Utility;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class LoadGameplayState : IEnterState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IDataTransferService _dataTransferService;
    private readonly IGameplayUiFactory _gameplayUiFactory;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly GameStatesFactory _gameStatesFactory;
    private readonly ISaveStorageService _saveStorageService;
    private readonly IStartingResourcesProvider _startingResourcesProvider;
    private readonly ILogService _logService;

    private readonly PersistentProgressService _persistentProgressService;

    private readonly List<IGameplayInit> _onLoadInit;
    private readonly List<IGameplayInitAsync> _onLoadInitAsync;

    public LoadGameplayState(IDataTransferService dataTransferService, IGameplayUiFactory gameplayUiFactory,
      GameStateMachine gameStateMachine, GameplayStateMachine gameplayStateMachine, GameStatesFactory gameStatesFactory,
      List<IGameplayInit> onLoadInit, PersistentProgressService persistentProgressService,
      ISaveStorageService saveStorageService,
      ILogService logService, IStartingResourcesProvider startingResourcesProvider,
      List<IGameplayInitAsync> onLoadInitAsync)
    {
      _dataTransferService = dataTransferService;
      _gameplayUiFactory = gameplayUiFactory;
      _gameStateMachine = gameStateMachine;
      _gameplayStateMachine = gameplayStateMachine;
      _gameStatesFactory = gameStatesFactory;
      _onLoadInit = onLoadInit;
      _persistentProgressService = persistentProgressService;
      _saveStorageService = saveStorageService;
      _logService = logService;
      _startingResourcesProvider = startingResourcesProvider;
      _onLoadInitAsync = onLoadInitAsync;
    }

    public void Enter() =>
      InternalEnterAsync().Forget();

    private async UniTaskVoid InternalEnterAsync()
    {
      if (!_dataTransferService.TryGetData(out GameplaySettings settings))
      {
        settings = GameplaySettings.Default;
        _logService.LogError(GetType(), "Gameplay settings not found", new NullReferenceException());
      }

      await InitializeGameState(settings);
      await InitializeServices();
      await InitializeUI();
      RegisterGameplayStates();

      _gameStateMachine.Enter<GameplayState>();
    }

    private async UniTask InitializeGameState(GameplaySettings settings)
    {
      if (settings.SaveSlot != SaveSlot.None)
      {
        LoadResult result = await _saveStorageService.LoadGameAsync(settings.SaveSlot);

        if (result.LoadStatus == LoadStatus.Success)
        {
          _persistentProgressService.Initialize(result.GameStateData);
          return;
        }
      }

      CreateNewGameState(settings);
    }

    private void CreateNewGameState(GameplaySettings settings)
    {
      SessionInfo sessionInfo = new SessionInfo(settings.SessionName, settings.GameDifficulty,
        UniqueIdGenerator.GenerateUniqueIntId());

      Dictionary<ResourceKind, GameResourceData> initialResources = _startingResourcesProvider
        .GetInitialResources(settings.GameDifficulty)
        .ToDictionary(x => x.Kind, x => x);

      _persistentProgressService.Initialize(new GameStateData(sessionInfo, initialResources));
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

      foreach (IGameplayInit initializable in _onLoadInit)
        initializable.Initialize();

      foreach (IGameplayInitAsync initializable in _onLoadInitAsync)
        initializationTasks.Add(initializable.InitializeAsync());

      await UniTask.WhenAll(initializationTasks);
    }
  }
}