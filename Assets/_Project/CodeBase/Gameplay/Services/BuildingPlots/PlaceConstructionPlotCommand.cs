using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public readonly struct PlaceConstructionPlotCommand : ICommand
  {
    public ConstructionPlotType Type { get; }
    public List<Vector2Int> OccupiedCells { get; }

    public PlaceConstructionPlotCommand(ConstructionPlotType type, List<Vector2Int> occupiedCells)
    {
      Type = type;
      OccupiedCells = occupiedCells;
    }
  }
}