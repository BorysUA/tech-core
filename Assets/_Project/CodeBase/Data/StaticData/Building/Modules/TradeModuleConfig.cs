using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Building.Modules.Spaceport;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "SpaceportTradeModule",
    menuName = "ScriptableObjects/BuildingModules/TradeModule", order = 0)]
  public class TradeModuleConfig : BuildingModuleWithConditionsConfig
  {
    public TradeConfig TradeConfig;

    public override BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig)
    {
      SpaceportTradeModule spaceportTradeModule =
        (SpaceportTradeModule)instantiator.Invoke(typeof(SpaceportTradeModule));

      spaceportTradeModule.Setup(TradeConfig);
      return spaceportTradeModule;
    }
  }
}