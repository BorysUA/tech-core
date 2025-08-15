using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Buildings.Conditions;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "EventConsumptionCondition",
    menuName = "ScriptableObjects/OperationalConditions/EventConsumptionCondition", order = 0)]
  public class SpendPerEventConditionConfig : OperationalConditionConfig
  {
    public ResourceAmountData ResourcePerEvent;
    public override bool IsValidFor(BuildingModule module) => module is IResourceSpenderEventSource;

    protected override OperationalCondition InstantiateCondition(Func<Type, OperationalCondition> instantiator) =>
      instantiator.Invoke(typeof(SpendPerEventCondition));

    protected override void SetupCondition(OperationalCondition condition, BuildingModule module)
    {
      base.SetupCondition(condition, module);
      SpendPerEventCondition spendPerEventCondition = (SpendPerEventCondition)condition;
      IResourceSpenderEventSource resourceSpender = (IResourceSpenderEventSource)module;
      spendPerEventCondition.Setup(ResourcePerEvent, resourceSpender.ResourceSpendRequested);
    }
  }
}