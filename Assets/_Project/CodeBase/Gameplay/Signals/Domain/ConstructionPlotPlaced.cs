namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ConstructionPlotPlaced
  {
    public readonly int PlotId;

    public ConstructionPlotPlaced(int plotId)
    {
      PlotId = plotId;
    }
  }
}