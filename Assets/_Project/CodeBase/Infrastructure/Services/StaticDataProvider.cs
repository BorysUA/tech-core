using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.InteractionButtons;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Services.RemoteConfigsService;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class StaticDataProvider : IStaticDataProvider, IBootstrapInitAsync
  {
    private readonly ILogService _logService;
    private readonly RemoteConfigPatcher _patcher;

    private Dictionary<BuildingType, BuildingConfig> _buildings = new();
    private Dictionary<ResourceKind, ResourceConfig> _resources = new();
    private Dictionary<MeteoriteType, MeteoriteConfig> _meteorites = new();
    private Dictionary<ResourceDropType, ResourceDropConfig> _resourcesDrops = new();
    private Dictionary<ConstructionPlotType, ConstructionPlotConfig> _constructionPlots = new();
    private Dictionary<ActionType, BuildingActionButtonConfig> _buildingActionButtons = new();
    private Dictionary<BuildingIndicatorType, BuildingIndicatorConfig> _buildingStatusItems = new();
    private Dictionary<MeteoriteType, MeteoriteVFX> _meteoriteVFXs = new();

    private readonly Dictionary<(BuildingType, Type), BuildingModuleConfig> _indexedBuildingModules = new();

    private GameMap _gameMap;
    private MeteoriteSpawnerConfig _meteoriteSpawner;
    private BuildingsShopCatalog _buildingsShopCatalog;

    private readonly List<AsyncOperationHandle> _handles = new();

    public StaticDataProvider(ILogService logService, RemoteConfigPatcher patcher)
    {
      _logService = logService;
      _patcher = patcher;
    }

    public async UniTask InitializeAsync()
    {
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

    public IEnumerable<BuildingConfig> GetAllBuildings() =>
      _buildings.Values;

    public IEnumerable<ResourceConfig> GetAllResources() =>
      _resources.Values;

    public IEnumerable<MeteoriteConfig> GetAllMeteorites() =>
      _meteorites.Values;

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
      IList<BuildingConfig> buildingConfigs = await LoadConfigsAsync<BuildingConfig>(StaticDataAddress.Buildings);
      _buildings = buildingConfigs.ToDictionary(x => x.Type, x => x);

      IList<ResourceConfig> resourceConfigs = await LoadConfigsAsync<ResourceConfig>(StaticDataAddress.Resources);
      _resources = resourceConfigs.ToDictionary(x => x.Kind, x => x);

      IList<MeteoriteConfig> meteoriteConfigs = await LoadConfigsAsync<MeteoriteConfig>(StaticDataAddress.Meteorites);
      _meteorites = meteoriteConfigs.ToDictionary(x => x.Type, x => x);

      IList<BuildingActionButtonConfig> buttonsConfig =
        await LoadConfigsAsync<BuildingActionButtonConfig>(StaticDataAddress.BuildingActionButtons);
      _buildingActionButtons = buttonsConfig.ToDictionary(x => x.Type, x => x);

      IList<ConstructionPlotConfig> buildingPlotConfigs =
        await LoadConfigsAsync<ConstructionPlotConfig>(StaticDataAddress.ConstructionPlots);
      _constructionPlots = buildingPlotConfigs.ToDictionary(x => x.Type, x => x);

      IList<ResourceDropConfig> resourceDropConfigs =
        await LoadConfigsAsync<ResourceDropConfig>(StaticDataAddress.ResourceDrops);
      _resourcesDrops = resourceDropConfigs.ToDictionary(x => x.Type, x => x);

      IList<BuildingIndicatorConfig> buildingStatusItemConfigs =
        await LoadConfigsAsync<BuildingIndicatorConfig>(StaticDataAddress.BuildingIndicators);
      _buildingStatusItems = buildingStatusItemConfigs.ToDictionary(x => x.Type, x => x);

      MeteoriteVFXConfig meteoriteVFXConfig =
        await LoadConfigAsync<MeteoriteVFXConfig>(StaticDataAddress.MeteoriteVFXs);
      _meteoriteVFXs = meteoriteVFXConfig.MeteoriteVFX.ToDictionary(x => x.Type, x => x);

      _meteoriteSpawner = await LoadConfigAsync<MeteoriteSpawnerConfig>(StaticDataAddress.MeteoriteSpawner);
      _gameMap = await LoadConfigAsync<GameMap>(StaticDataAddress.GameMap);
      _buildingsShopCatalog = await LoadConfigAsync<BuildingsShopCatalog>(StaticDataAddress.BuildingsShopCatalog);
    }

    private async UniTask<T> LoadConfigAsync<T>(string address)
    {
      AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
      _handles.Add(handle);

      await handle.Task;

      if (handle.Status == AsyncOperationStatus.Succeeded)
        return handle.Result;

      _logService.LogError(GetType(), $"Failed to load config with specified address '{address}'");
      return default;
    }

    private async UniTask<IList<T>> LoadConfigsAsync<T>(string address)
    {
      AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(address);
      _handles.Add(handle);

      await handle.Task;

      if (handle.Status == AsyncOperationStatus.Succeeded)
        return handle.Result;

      _logService.LogError(GetType(), $"Failed to load configs with specified address '{address}'");
      return null;
    }
  }
}