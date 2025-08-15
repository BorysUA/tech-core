using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Buildings.Modules.Trade;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [CreateAssetMenu(fileName = "SpaceportTradeModule",
    menuName = "ScriptableObjects/BuildingModules/TradeModule", order = 0)]
  public class TradeModuleConfig : BuildingModuleConfig, IModuleProgressFactory
  {
    public TradeConfig TradeConfig;

    public override Type ModuleType => typeof(TradeModule);

    protected override BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator) =>
      instantiator.Invoke(typeof(TradeModule));

    protected override void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      base.SetupModule(module, buildingConfig);
      TradeModule tradeModule = (TradeModule)module;
      tradeModule.Setup(TradeConfig);
    }

    public (Type moduleType, IModuleData data) CreateInitialData()
    {
      return (typeof(TradeModule), new TradeData(0));
    }
  }
}