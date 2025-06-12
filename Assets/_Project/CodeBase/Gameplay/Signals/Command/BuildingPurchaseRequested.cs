using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Command
{
  public class BuildingPurchaseRequested
  {
    public BuildingType Type { get; private set; }

    public BuildingPurchaseRequested(BuildingType type)
    {
      Type = type;
    }
  }
}