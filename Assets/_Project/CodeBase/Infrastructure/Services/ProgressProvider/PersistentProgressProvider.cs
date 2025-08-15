using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Utility;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services.ProgressProvider
{
  public class PersistentProgressProvider : IProgressReader, IProgressWriter, IProgressSaver, IGameplayInitAsync
  {
    private GameStateModel _gameStateModel;

    private readonly ISaveStorageService _saveStorageService;
    private readonly IDataTransferService _dataTransferService;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ILogService _logService;

    IGameStateReader IProgressReader.GameStateModel => _gameStateModel;
    IGameStateWriter IProgressWriter.GameStateModel => _gameStateModel;
    IGameStateSaver IProgressSaver.GameStateModel => _gameStateModel;

    public PersistentProgressProvider(ISaveStorageService saveStorageService, IDataTransferService dataTransferService,
      ILogService logService, IStaticDataProvider staticDataProvider)
    {
      _saveStorageService = saveStorageService;
      _dataTransferService = dataTransferService;
      _logService = logService;
      _staticDataProvider = staticDataProvider;
    }

    public async UniTask InitializeAsync()
    {
      if (!_dataTransferService.TryGetData(out GameplaySettings settings))
      {
        settings = GameplaySettings.Default;
        _logService.LogError(GetType(), "Gameplay settings not found", new NullReferenceException());
      }

      await InitializeGameState(settings);
    }

    private async UniTask InitializeGameState(GameplaySettings settings)
    {
      if (settings.SaveSlot != SaveSlot.None)
      {
        LoadResult result = await _saveStorageService.LoadGameAsync(settings.SaveSlot);

        if (result.LoadStatus == LoadStatus.Success)
        {
          _gameStateModel = new GameStateModel(result.GameStateData);
          return;
        }
      }

      CreateNewGameState(settings);
    }

    private void CreateNewGameState(GameplaySettings settings)
    {
      SessionInfo sessionInfo = new SessionInfo(settings.SessionName, settings.GameDifficulty,
        UniqueIdGenerator.GenerateUniqueIntId());

      GameStartProfile gameStartProfile = _staticDataProvider.GetGameStartProfile(settings.GameDifficulty);

      Dictionary<ResourceKind, GameResourceData> initialResources =
        gameStartProfile.Resources.ToDictionary(x => x.Kind,
          x => new GameResourceData(x.Kind, x.Amount, x.Capacity));

      GameStateData clearState = new GameStateData(sessionInfo, initialResources);
      _gameStateModel = new GameStateModel(clearState);
    }
  }
}