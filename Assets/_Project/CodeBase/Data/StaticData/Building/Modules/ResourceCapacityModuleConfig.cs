using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Modules;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "CapacityModule", menuName = "ScriptableObjects/BuildingModules/CapacityModule",
    order = 0)]
  public class ResourceCapacityModuleConfig : BuildingModuleConfig
  {
    public CapacityEffect CapacityEffect;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      CapacityModule capacityModule = (CapacityModule)instantiator.Invoke(typeof(CapacityModule));
      capacityModule.Setup(CapacityEffect);
      return capacityModule;
    }
  }

  [Serializable]
  public struct CapacityEffect
  {
    public ResourceAmountData AdditionalCapacity;
    public bool FillOnAdd;
  }
}