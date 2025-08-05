using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.SelfDestruction;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "SelfDestructionModule",
    menuName = "ScriptableObjects/BuildingModules/SelfDestructionModule", order = 0)]
  public class SelfDestructionModuleConfig : BuildingModuleConfig
  {
    public float RefundRatio;

    public override Type ModuleType => typeof(SelfDestructionModule);

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(SelfDestructionModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      SelfDestructionModule selfDestructionModule = (SelfDestructionModule)module;
      selfDestructionModule.Setup(RefundRatio, buildingConfig.Price);
    }
  }
}