using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public class ModuleContextResolver
  {
    private readonly IProgressWriter _progressWriter;
    private readonly IStaticDataProvider _staticDataProvider;

    public ModuleContextResolver(IProgressWriter progressWriter, IStaticDataProvider staticDataProvider)
    {
      _progressWriter = progressWriter;
      _staticDataProvider = staticDataProvider;
    }

    public ModuleContext<TConfig, TData> Resolve<TConfig, TData>(int buildingId)
      where TConfig : BuildingModuleConfig
      where TData : class, IModuleData
    {
      IBuildingDataWriter building = _progressWriter.GameStateModel.WriteOnlyBuildings.Get(buildingId);

      TConfig config = _staticDataProvider.GetModuleConfig<TConfig>(building.Type)
                       ?? throw new InvalidOperationException($"TradeModuleConfig not found for {building.Type}");

      if (!building.ModulesProgress.TryGetValue(config.ModuleType, out IModuleData raw) || raw is not TData data)
        throw new InvalidOperationException(
          $"[{buildingId}] {typeof(TData).Name} not found (key={config.ModuleType.Name})");

      return new ModuleContext<TConfig, TData>(building, config, data);
    }

    public bool TryResolve<TConfig, TData>(int buildingId, out ModuleContext<TConfig, TData> moduleContext)
      where TConfig : BuildingModuleConfig
      where TData : class, IModuleData
    {
      try
      {
        moduleContext = Resolve<TConfig, TData>(buildingId);
        return true;
      }
      catch
      {
        moduleContext = default;
        return false;
      }
    }
  }
}