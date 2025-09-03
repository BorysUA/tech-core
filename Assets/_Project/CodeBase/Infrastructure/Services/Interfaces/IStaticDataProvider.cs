using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.InteractionButtons;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IStaticDataProvider
  {
    BuildingConfig GetBuildingConfig(BuildingType buildingType);
    ResourceConfig GetResourceConfig(ResourceKind resourceKind);
    MeteoriteConfig GetMeteoriteConfig(MeteoriteType meteoriteType);
    MeteoriteSpawnerConfig GetMeteoriteSpawnerConfig();
    ResourceDropConfig GetResourceDropConfig(ResourceDropType resourceDropType);
    IEnumerable<ConstructionPlotConfig> GetAllBuildingPlots();
    ConstructionPlotConfig GetConstructionPlotConfig(ConstructionPlotType type);
    IEnumerable<BuildingActionButtonConfig> GetAllBuildingActionButtons();
    BuildingActionButtonConfig GetBuildingActionButtonConfig(ActionType actionType);
    BuildingIndicatorConfig GetBuildingIndicatorConfig(BuildingIndicatorType indicatorType);
    MeteoriteVFX GetMeteoriteVFXs(MeteoriteType meteoriteType);
    IEnumerable<MapEntityData> GetMapEntities();
    BuildingsShopCatalog GetBuildingsShopCatalog();
    T GetModuleConfig<T>(BuildingType buildingType) where T : BuildingModuleConfig;
    GameStartProfile GetGameStartProfile(GameDifficulty gameDifficulty);
    IReadOnlyList<ActionType> GetActionButtonsOrder();
  }
}