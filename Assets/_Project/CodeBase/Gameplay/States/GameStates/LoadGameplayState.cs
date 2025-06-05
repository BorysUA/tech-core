using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.States.GameplayStates;
using _Project.CodeBase.Gameplay.States.GameplayStates.Placement;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.States.GameStates
{
  public class LoadGameplayState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IDataTransferService _dataTransferService;
    private readonly IGameplayUiFactory _gameplayUiFactory;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly GameStatesFactory _gameStatesFactory;
    private readonly ISaveStorageService _saveStorageService;
    private readonly ILogService _logService;

    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IProgressService _progressService;

    private readonly List<IOnLoadInitializable> _onLoadInitializables;

    public LoadGameplayState(IDataTransferService dataTransferService, IGameplayUiFactory gameplayUiFactory,
      GameStateMachine gameStateMachine, GameplayStateMachine gameplayStateMachine, GameStatesFactory gameStatesFactory,
      List<IOnLoadInitializable> onLoadInitializables, IStaticDataProvider staticDataProvider,
      IProgressService progressService, ISaveStorageService saveStorageService, ILogService logService)
    {
      _dataTransferService = dataTransferService;
      _gameplayUiFactory = gameplayUiFactory;
      _gameStateMachine = gameStateMachine;
      _gameplayStateMachine = gameplayStateMachine;
      _gameStatesFactory = gameStatesFactory;
      _onLoadInitializables = onLoadInitializables;
      _staticDataProvider = staticDataProvider;
      _progressService = progressService;
      _saveStorageService = saveStorageService;
      _logService = logService;
    }

    public async void Enter()
    {
      if (!_dataTransferService.TryGetData(out GameplaySettings settings))
      {
        settings = GameplaySettings.Default;
        _logService.LogError(GetType(), "Gameplay settings not found", new NullReferenceException());
      }

      await InitializeGameState(settings.SaveSlot);
      InitializeServices();
      await _gameplayUiFactory.CreateHud();
      InitializeGameplayStates();

      _gameStateMachine.Enter<GameplayState>();
    }

    public void Exit()
    {
    }

    private async UniTask InitializeGameState(SaveSlot saveSlot)
    {
      if (saveSlot != SaveSlot.None)
      {
        LoadResult result = await _saveStorageService.LoadGameAsync(saveSlot);

        if (result.LoadStatus == LoadStatus.Success)
        {
          _progressService.GameStateProxy.Initialize(result.GameStateData);
          return;
        }
      }

      CreateNewGameState();
    }

    private void CreateNewGameState()
    {
      IEnumerable<ResourceConfig> resourcesData = _staticDataProvider.GetAllResources();

      SessionInfo sessionInfo = new SessionInfo("Mars colony", GameDifficulty.Easy);

      Dictionary<ResourceKind, GameResourceData> gameResources = resourcesData
        .ToDictionary(resource => resource.Kind,
          resource => new GameResourceData(resource.Kind, 300, resource.Capacity));

      _progressService.GameStateProxy.Initialize(new GameStateData(sessionInfo, gameResources));
    }


    private void InitializeGameplayStates()
    {
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<PlaceBuildingState>());
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<DefaultGameplayState>());
      _gameplayStateMachine.RegisterState(_gameStatesFactory.CreateState<PlaceConstructionPlotState>());
    }

    private void InitializeServices()
    {
      foreach (IOnLoadInitializable initializable in _onLoadInitializables)
        initializable.Initialize();
    }
  }
}