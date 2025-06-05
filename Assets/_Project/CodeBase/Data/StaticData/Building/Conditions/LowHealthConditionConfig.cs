using System;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "LowHealthCondition",
    menuName = "ScriptableObjects/OperationalConditions/LowHealthCondition", order = 0)]
  public class LowHealthConditionConfig : OperationalConditionConfig
  {
    public float HealthThreshold;

    public override bool IsValidFor(IConditionBoundModule module) => module is HealthModule;

    public override OperationalCondition CreateOperationalCondition(Func<Type, OperationalCondition> instantiator,
      IConditionBoundModule module)
    {
      LowHealthCondition condition = (LowHealthCondition)instantiator.Invoke(typeof(LowHealthCondition));

      HealthModule healthModule = (HealthModule)module;

      condition.Setup(HealthThreshold, healthModule.Ratio);
      return condition;
    }
  }
}