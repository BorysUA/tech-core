using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ConstructionPlotPlaced
  {
    public readonly ConstructionPlotType Type;

    public ConstructionPlotPlaced(ConstructionPlotType type)
    {
      Type = type;
    }
  }
}