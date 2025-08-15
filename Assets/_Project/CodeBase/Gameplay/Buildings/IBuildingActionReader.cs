using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using R3;

namespace _Project.CodeBase.Gameplay.Buildings
{
  public interface IBuildingActionReader
  {
    public int Id { get; }
    public IEnumerable<IBuildingActionsProvider> Actions { get; }
    public Observable<Unit> Destroyed { get; }
  }
}