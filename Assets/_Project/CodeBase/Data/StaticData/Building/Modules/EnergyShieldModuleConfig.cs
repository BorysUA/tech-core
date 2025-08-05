using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.EnergyShield;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "EnergyShieldModule",
    menuName = "ScriptableObjects/BuildingModules/EnergyShieldModule", order = 0)]
  public class EnergyShieldModuleConfig : BuildingModuleConfig
  {
    public ShieldConfig ShieldConfig;

    public override Type ModuleType => typeof(EnergyShieldModule);

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(EnergyShieldModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      EnergyShieldModule energyShieldModule = (EnergyShieldModule)module;
      energyShieldModule.Setup(ShieldConfig);
    }
  }
}