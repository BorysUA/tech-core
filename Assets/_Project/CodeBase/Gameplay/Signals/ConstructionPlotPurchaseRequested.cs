using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;

namespace _Project.CodeBase.Gameplay.Signals
{
  public class ConstructionPlotPurchaseRequested
  {
    public ConstructionPlotType Type { get; private set; }

    public ConstructionPlotPurchaseRequested(ConstructionPlotType type)
    {
      Type = type;
    }
  }
}