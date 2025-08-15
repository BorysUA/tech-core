using System;
using _Project.CodeBase.Gameplay.Buildings.Conditions;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Buildings.Modules.Health;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "LowHealthCondition",
    menuName = "ScriptableObjects/OperationalConditions/LowHealthCondition", order = 0)]
  public class LowHealthConditionConfig : OperationalConditionConfig
  {
    public float HealthThreshold;

    protected override OperationalCondition InstantiateCondition(Func<Type, OperationalCondition> instantiator) =>
      instantiator.Invoke(typeof(LowHealthCondition));

    public override bool IsValidFor(BuildingModule module) => module is HealthModule;

    protected override void SetupCondition(OperationalCondition condition, BuildingModule module)
    {
      base.SetupCondition(condition, module);
      LowHealthCondition lowHealthCondition = (LowHealthCondition)condition;
      HealthModule healthModule = (HealthModule)module;
      lowHealthCondition.Setup(HealthThreshold, healthModule.Ratio);
    }
  }
}