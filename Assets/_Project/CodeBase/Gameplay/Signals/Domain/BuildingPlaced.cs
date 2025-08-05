using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct BuildingPlaced
  {
    public readonly int BuildingId;

    public BuildingPlaced(int buildingId)
    {
      BuildingId = buildingId;
    }
  }
}