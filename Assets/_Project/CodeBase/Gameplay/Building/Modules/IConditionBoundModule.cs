using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Building.Conditions;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public interface IConditionBoundModule : IBuildingIndicatorsProvider
  {
    public void Setup(List<OperationalCondition> conditions);
    public ReadOnlyReactiveProperty<bool> IsOperational { get; }
  }
}