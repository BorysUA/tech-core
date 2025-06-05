using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.Progress
{
  [Serializable]
  public class ConstructionPlotData
  {
    public string Id;
    public ConstructionPlotType Type;
    public List<Vector2Int> OccupiedCells;

    public ConstructionPlotData(string id, ConstructionPlotType type, List<Vector2Int> occupiedCells)
    {
      Id = id;
      Type = type;
      OccupiedCells = occupiedCells;
    }
  }
}