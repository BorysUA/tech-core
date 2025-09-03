using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Command
{
  public readonly struct BuildingPurchaseRequested
  {
    public BuildingType Type { get; }

    public BuildingPurchaseRequested(BuildingType type)
    {
      Type = type;
    }
  }
}