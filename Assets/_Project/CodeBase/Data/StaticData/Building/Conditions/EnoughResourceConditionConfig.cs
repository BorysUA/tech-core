using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "EnoughResourceCondition",
    menuName = "ScriptableObjects/OperationalConditions/EnoughResourceCondition", order = 0)]
  public class EnoughResourceConditionConfig : OperationalConditionConfig
  {
    public ResourceAmountData RequiredResources;
    public BuildingIndicatorType IndicatorType;

    public override bool IsValidFor(IConditionBoundModule buildingModule) => buildingModule is ResourceConsumerModule;

    public override OperationalCondition CreateOperationalCondition(Func<Type, OperationalCondition> instantiator,
      IConditionBoundModule module)
    {
      EnoughResourceCondition condition = (EnoughResourceCondition)instantiator.Invoke(typeof(EnoughResourceCondition));

      condition!.Setup(IndicatorType, RequiredResources);
      return condition;
    }
  }
}