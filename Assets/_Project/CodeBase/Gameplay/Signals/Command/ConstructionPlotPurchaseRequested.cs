using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Command
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