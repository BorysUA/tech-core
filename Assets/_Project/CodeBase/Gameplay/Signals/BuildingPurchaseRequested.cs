using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals
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