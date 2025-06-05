using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public readonly struct PlaceBuildingCommand : ICommand
  {
    public BuildingType Type { get; }
    public int Level { get; }
    public List<Vector2Int> OccupiedCells { get; }

    public PlaceBuildingCommand(BuildingType type, int level, List<Vector2Int> occupiedCells)
    {
      Type = type;
      Level = level;
      OccupiedCells = occupiedCells;
    }
  }
}