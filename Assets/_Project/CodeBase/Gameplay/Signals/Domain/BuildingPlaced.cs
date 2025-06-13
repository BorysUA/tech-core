namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct BuildingPlaced
  {
    public readonly _Project.CodeBase.Gameplay.Constants.BuildingType Type;
    public readonly int Level;

    public BuildingPlaced(_Project.CodeBase.Gameplay.Constants.BuildingType type, int level)
    {
      Type = type;
      Level = level;
    }
  }
}
