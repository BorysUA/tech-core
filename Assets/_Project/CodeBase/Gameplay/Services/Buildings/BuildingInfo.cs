using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public readonly struct BuildingInfo
  {
    public BuildingType Type { get; }
    public BuildingCategory Category { get; }
    public Vector2Int Size { get; }

    public BuildingInfo(BuildingType type, BuildingCategory category, Vector2Int size)
    {
      Type = type;
      Category = category;
      Size = size;
    }
  }
}