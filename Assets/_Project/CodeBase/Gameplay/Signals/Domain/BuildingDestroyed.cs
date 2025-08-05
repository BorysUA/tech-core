using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct BuildingDestroyed
  {
    public readonly int Id;
    public readonly BuildingType Type;
    public readonly int Level;

    public BuildingDestroyed(int id, BuildingType type, int level)
    {
      Id = id;
      Type = type;
      Level = level;
    }
  }
}