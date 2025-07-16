using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "ConstantResourceConsumption",
    menuName = "ScriptableObjects/OperationalConditions/ConstantResourceConsumption", order = 0)]
  public class SpendPerTickConditionConfig : OperationalConditionConfig
  {
    public ResourceAmountData RequiredResources;
    public ResourceFlowConfig ResourceFlow;

    protected override OperationalCondition InstantiateCondition(Func<Type, OperationalCondition> instantiator) =>
      instantiator.Invoke(typeof(SpendPerTickCondition));

    public override bool IsValidFor(BuildingModule buildingModule) => true;

    protected override void SetupCondition(OperationalCondition condition, BuildingModule module)
    {
      base.SetupCondition(condition, module);
      SpendPerTickCondition spendPerTickCondition =
        (SpendPerTickCondition)condition;
      spendPerTickCondition.Setup(RequiredResources, ResourceFlow);
    }
  }
}