using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.InteractionButtons;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Services.RemoteConfigsService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services.AssetsPipeline
{
  public class StaticDataProvider : IStaticDataProvider, IBootstrapInitAsync
  {
    private readonly RemoteConfigPatcher _patcher;
    private readonly IAssetProvider _assetProvider;

    private Dictionary<BuildingType, BuildingConfig> _buildings = new();
    private Dictionary<ResourceKind, ResourceConfig> _resources = new();
    private Dictionary<MeteoriteType, MeteoriteConfig> _meteorites = new();
    private Dictionary<ResourceDropType, ResourceDropConfig> _resourcesDrops = new();
    private Dictionary<ConstructionPlotType, ConstructionPlotConfig> _constructionPlots = new();
    private Dictionary<ActionType, BuildingActionButtonConfig> _buildingActionButtons = new();
    private Dictionary<BuildingIndicatorType, BuildingIndicatorConfig> _buildingStatusItems = new();
    private Dictionary<MeteoriteType, MeteoriteVFX> _meteoriteVFXs = new();
    private Dictionary<GameDifficulty, GameStartProfile> _gameStartProfiles = new();

    private readonly Dictionary<(BuildingType, Type), BuildingModuleConfig> _indexedBuildingModules = new();

    private GameMap _gameMap;
    private MeteoriteSpawnerConfig _meteoriteSpawner;
    private BuildingsShopCatalog _buildingsShopCatalog;

    public StaticDataProvider(RemoteConfigPatcher patcher, IAssetProvider assetProvider)
    {
      _patcher = patcher;
      _assetProvider = assetProvider;
    }

    public async UniTask InitializeAsync()
    {
      await _assetProvider.WhenReady;
      await DownloadAllConfigsAsync();
      await _patcher.WhenReady;
      ApplyRemotePatches();
      Indexing();
    }

    public T GetModuleConfig<T>(BuildingType buildingType) where T : BuildingModuleConfig
    {
      return _indexedBuildingModules.TryGetValue((buildingType, typeof(T)), out BuildingModuleConfig config)
        ? (T)config
        : null;
    }

    public BuildingConfig GetBuildingConfig(BuildingType buildingType) =>
      _buildings.GetValueOrDefault(buildingType);

    public IEnumerable<MapEntityData> GetMapEntities()
      => _gameMap.Entities;

    public BuildingsShopCatalog GetBuildingsShopCatalog()
      => _buildingsShopCatalog;

    public ResourceConfig GetResourceConfig(ResourceKind resourceKind) =>
      _resources.GetValueOrDefault(resourceKind);

    public GameStartProfile GetGameStartProfile(GameDifficulty gameDifficulty) =>
      _gameStartProfiles.GetValueOrDefault(gameDifficulty);

    public BuildingIndicatorConfig GetBuildingIndicatorConfig(BuildingIndicatorType indicatorType) =>
      _buildingStatusItems.GetValueOrDefault(indicatorType);

    public MeteoriteConfig GetMeteoriteConfig(MeteoriteType meteoriteType) =>
      _meteorites.GetValueOrDefault(meteoriteType);

    public MeteoriteVFX GetMeteoriteVFXs(MeteoriteType meteoriteType) =>
      _meteoriteVFXs.GetValueOrDefault(meteoriteType);

    public ResourceDropConfig GetResourceDropConfig(ResourceDropType resourceDropType) =>
      _resourcesDrops.GetValueOrDefault(resourceDropType);

    public IEnumerable<ConstructionPlotConfig> GetAllBuildingPlots() =>
      _constructionPlots.Values;

    public ConstructionPlotConfig GetConstructionPlotConfig(ConstructionPlotType constructionPlotType) =>
      _constructionPlots.GetValueOrDefault(constructionPlotType);

    public IEnumerable<BuildingActionButtonConfig> GetAllBuildingActionButtons() =>
      _buildingActionButtons.Values;

    public BuildingActionButtonConfig GetBuildingActionButtonConfig(ActionType actionType) =>
      _buildingActionButtons.GetValueOrDefault(actionType);

    public MeteoriteSpawnerConfig GetMeteoriteSpawnerConfig() =>
      _meteoriteSpawner;

    private void ApplyRemotePatches()
    {
      _meteoriteSpawner = _patcher.CreatePatchedProxy(_meteoriteSpawner);
    }

    private void Indexing()
    {
      foreach (var (buildingType, buildingConfig) in _buildings)
      {
        foreach (BuildingModuleConfig moduleConfig in buildingConfig.BuildingsModules)
          _indexedBuildingModules[(buildingType, moduleConfig.GetType())] = moduleConfig;
      }
    }

    private async UniTask DownloadAllConfigsAsync()
    {
      IList<BuildingConfig> buildingConfigs =
        await _assetProvider.LoadAssetsAsync<BuildingConfig>(StaticDataAddress.Buildings);
      _buildings = buildingConfigs.ToDictionary(x => x.Type, x => x);

      IList<GameStartProfile> gameStartProfiles =
        await _assetProvider.LoadAssetsAsync<GameStartProfile>(StaticDataAddress.GameStartProfiles);
      _gameStartProfiles = gameStartProfiles.ToDictionary(x => x.Difficulty, x => x);

      IList<ResourceConfig> resourceConfigs =
        await _assetProvider.LoadAssetsAsync<ResourceConfig>(StaticDataAddress.Resources);
      _resources = resourceConfigs.ToDictionary(x => x.Kind, x => x);

      IList<MeteoriteConfig> meteoriteConfigs =
        await _assetProvider.LoadAssetsAsync<MeteoriteConfig>(StaticDataAddress.Meteorites);
      _meteorites = meteoriteConfigs.ToDictionary(x => x.Type, x => x);

      IList<BuildingActionButtonConfig> buttonsConfig =
        await _assetProvider.LoadAssetsAsync<BuildingActionButtonConfig>(StaticDataAddress.BuildingActionButtons);
      _buildingActionButtons = buttonsConfig.ToDictionary(x => x.Type, x => x);

      IList<ConstructionPlotConfig> buildingPlotConfigs =
        await _assetProvider.LoadAssetsAsync<ConstructionPlotConfig>(StaticDataAddress.ConstructionPlots);
      _constructionPlots = buildingPlotConfigs.ToDictionary(x => x.Type, x => x);

      IList<ResourceDropConfig> resourceDropConfigs =
        await _assetProvider.LoadAssetsAsync<ResourceDropConfig>(StaticDataAddress.ResourceDrops);
      _resourcesDrops = resourceDropConfigs.ToDictionary(x => x.Type, x => x);

      IList<BuildingIndicatorConfig> buildingStatusItemConfigs =
        await _assetProvider.LoadAssetsAsync<BuildingIndicatorConfig>(StaticDataAddress.BuildingIndicators);
      _buildingStatusItems = buildingStatusItemConfigs.ToDictionary(x => x.Type, x => x);

      MeteoriteVFXConfig meteoriteVFXConfig =
        await _assetProvider.LoadAssetAsync<MeteoriteVFXConfig>(StaticDataAddress.MeteoriteVFXs);
      _meteoriteVFXs = meteoriteVFXConfig.MeteoriteVFX.ToDictionary(x => x.Type, x => x);

      _meteoriteSpawner =
        await _assetProvider.LoadAssetAsync<MeteoriteSpawnerConfig>(StaticDataAddress.MeteoriteSpawner);
      _gameMap = await _assetProvider.LoadAssetAsync<GameMap>(StaticDataAddress.GameMap);
      _buildingsShopCatalog =
        await _assetProvider.LoadAssetAsync<BuildingsShopCatalog>(StaticDataAddress.BuildingsShopCatalog);
    }
  }
}