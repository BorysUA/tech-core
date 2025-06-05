using System;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "ResourceConsumerModule",
    menuName = "ScriptableObjects/BuildingModules/ResourceConsumerModule",
    order = 0)]
  public class ResourceConsumerModuleConfig : BuildingModuleWithConditionsConfig
  {
    public ResourceFlowConfig ResourceFlowConfig;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      ResourceConsumerModule resourceConsumerModule =
        (ResourceConsumerModule)instantiator.Invoke(typeof(ResourceConsumerModule));

      resourceConsumerModule.Setup(ResourceFlowConfig);

      return resourceConsumerModule;
    }
  }
}