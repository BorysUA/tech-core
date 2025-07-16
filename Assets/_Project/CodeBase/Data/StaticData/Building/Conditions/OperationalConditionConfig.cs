using System;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  public abstract class OperationalConditionConfig : ScriptableObject
  {
    [field: SerializeField] public BuildingIndicatorType IndicatorType { get; private set; }
    [field: SerializeField] public bool IsGlobalCondition { get; private set; }

    public OperationalCondition CreateOperationalCondition(Func<Type, OperationalCondition> instantiator,
      BuildingModule module)
    {
      OperationalCondition condition = InstantiateCondition(instantiator);
      SetupCondition(condition, module);
      return condition;
    }

    public abstract bool IsValidFor(BuildingModule module);

    protected abstract OperationalCondition InstantiateCondition(Func<Type, OperationalCondition> instantiator);

    protected virtual void SetupCondition(OperationalCondition condition, BuildingModule module)
    {
      condition.Setup(module.IsModuleWorking, IndicatorType);
    }
  }
}