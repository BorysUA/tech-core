using System;
using _Project.CodeBase.Data.Progress.ResourceData;
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

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      SelfDestructionModule module = (SelfDestructionModule)instantiator.Invoke(typeof(SelfDestructionModule));

      module.Setup(RefundRatio, buildingConfig.Price);

      return module;
    }
  }
}