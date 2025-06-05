using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "ResourceReservationModule",
    menuName = "ScriptableObjects/BuildingModules/ResourceReservationModule", order = 0)]
  public class ResourceReservationModuleConfig : BuildingModuleWithConditionsConfig
  {
    public ResourceAmountData ResourceAmountData;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      ResourceReservationModule module =
        (ResourceReservationModule)instantiator.Invoke(typeof(ResourceReservationModule));

      module.Setup(ResourceAmountData);
      return module;
    }
  }
}