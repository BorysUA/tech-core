using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Gameplay.Buildings.Modules
{
  public readonly struct ModuleContext<TConfig, TData> where TConfig : BuildingModuleConfig where TData : IModuleData
  {
    public IBuildingDataWriter Building { get; }
    public TConfig Config { get; }
    public TData Data { get; }

    public ModuleContext(IBuildingDataWriter building, TConfig config, TData data)
    {
      Config = config;
      Data = data;
      Building = building;
    }
  }
}