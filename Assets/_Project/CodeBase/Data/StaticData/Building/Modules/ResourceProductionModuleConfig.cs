using System;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "ProductionModule", menuName = "ScriptableObjects/BuildingModules/ProductionModule",
    order = 0)]
  public class ResourceProductionModuleConfig : BuildingModuleConfig
  {
    public ResourceFlowConfig ResourceFlowConfig;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      ResourceProductionModule resourceProductionModule =
        (ResourceProductionModule)instantiator.Invoke(typeof(ResourceProductionModule));
      resourceProductionModule.Setup(ResourceFlowConfig);
      return resourceProductionModule;
    }
  }
}