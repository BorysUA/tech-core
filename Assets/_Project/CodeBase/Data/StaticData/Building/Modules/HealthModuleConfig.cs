using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "HealthModule", menuName = "ScriptableObjects/BuildingModules/HealthModule", order = 0)]
  public class HealthModuleConditionsConfig : BuildingModuleConfig
  {
    public HealthConfig HealthConfig;

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(HealthModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      HealthModule healthModule = (HealthModule)module;
      healthModule.Setup(HealthConfig);
    }
  }
}