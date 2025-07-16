using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Spaceport;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "SpaceportTradeModule",
    menuName = "ScriptableObjects/BuildingModules/TradeModule", order = 0)]
  public class TradeModuleConfig : BuildingModuleConfig
  {
    public TradeConfig TradeConfig;

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(SpaceportTradeModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      SpaceportTradeModule spaceportTradeModule = (SpaceportTradeModule)module;
      spaceportTradeModule.Setup(TradeConfig);
    }
  }
}