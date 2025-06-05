using System;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  public abstract class OperationalConditionConfig : ScriptableObject
  {
    public abstract bool IsValidFor(IConditionBoundModule module);

    public abstract OperationalCondition CreateOperationalCondition(Func<Type, OperationalCondition> instantiator,
      IConditionBoundModule module);
  }
}