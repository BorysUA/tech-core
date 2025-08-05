using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Building.Actions.Common
{
  public interface IBuildingActionsProvider
  {
    IReadOnlyList<IBuildingAction> Actions { get; }
  }
}