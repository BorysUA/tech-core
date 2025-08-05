using System;
using _Project.CodeBase.Data.StaticData.Resource;
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

    public override Type ModuleType => typeof(ResourceProductionModule);

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(ResourceProductionModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      ResourceProductionModule resourceProductionModule = (ResourceProductionModule)module;
      resourceProductionModule.Setup(ResourceFlowConfig);
    }
  }
}