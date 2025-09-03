using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Command
{
  public readonly struct ConstructionPlotPurchaseRequested
  {
    public ConstructionPlotType Type { get; }

    public ConstructionPlotPurchaseRequested(ConstructionPlotType type)
    {
      Type = type;
    }
  }
}