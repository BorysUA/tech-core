using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "CapacityModule", menuName = "ScriptableObjects/BuildingModules/CapacityModule",
    order = 0)]
  public class ResourceCapacityModuleConfig : BuildingModuleConfig
  {
    public CapacityEffect CapacityEffect;

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(CapacityModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      CapacityModule capacityModule = (CapacityModule)module;
      capacityModule.Setup(CapacityEffect);
    }
  }

  [Serializable]
  public struct CapacityEffect
  {
    public ResourceAmountData AdditionalCapacity;
    public bool FillOnAdd;
  }
}