using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public interface IBuildingIndicatorsProvider
  {
    public IEnumerable<IBuildingIndicatorSource> Indicators { get; }
  }
}