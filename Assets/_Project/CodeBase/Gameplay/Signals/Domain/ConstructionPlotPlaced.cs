namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ConstructionPlotPlaced
  {
    public readonly _Project.CodeBase.Gameplay.Constants.ConstructionPlotType Type;

    public ConstructionPlotPlaced(_Project.CodeBase.Gameplay.Constants.ConstructionPlotType type)
    {
      Type = type;
    }
  }
}
