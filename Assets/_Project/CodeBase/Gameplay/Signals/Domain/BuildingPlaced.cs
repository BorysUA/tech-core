using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct BuildingPlaced
  {
    public readonly BuildingType Type;
    public readonly int Level;

    public BuildingPlaced(BuildingType type, int level)
    {
      Type = type;
      Level = level;
    }
  }
}
