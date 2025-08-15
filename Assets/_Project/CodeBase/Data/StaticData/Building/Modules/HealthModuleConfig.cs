using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Buildings.Modules.Health;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "HealthModule", menuName = "ScriptableObjects/BuildingModules/HealthModule", order = 0)]
  public class HealthModuleConfig : BuildingModuleConfig, IModuleProgressFactory
  {
    public HealthConfig HealthConfig;

    public override Type ModuleType => typeof(HealthModule);

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(HealthModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      HealthModule healthModule = (HealthModule)module;
      healthModule.Setup(HealthConfig);
    }

    public (Type moduleType, IModuleData data) CreateInitialData()
    {
      return (typeof(HealthModule), new HealthData(HealthConfig.Max));
    }
  }
}