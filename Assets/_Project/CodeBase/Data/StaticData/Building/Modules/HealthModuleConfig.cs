using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "HealthModule", menuName = "ScriptableObjects/BuildingModules/HealthModule", order = 0)]
  public class HealthModuleConditionsConfig : BuildingModuleWithConditionsConfig
  {
    public HealthConfig HealthConfig;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      HealthModule healthModule =
        (HealthModule)instantiator.Invoke(typeof(HealthModule));

      healthModule.Setup(HealthConfig);
      return healthModule;
    }
  }
}