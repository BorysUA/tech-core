using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Buildings.Actions.Common
{
  public interface IBuildingActionsProvider
  {
    IReadOnlyList<IBuildingAction> Actions { get; }
  }
}