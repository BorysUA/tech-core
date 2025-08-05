using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ConstructionPlotPlaced
  {
    public readonly string PlotId;

    public ConstructionPlotPlaced(string plotId)
    {
      PlotId = plotId;
    }
  }
}