using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IPlotDataReader
  {
    public string Id { get; }
    public ConstructionPlotType Type { get; }
    public List<Vector2Int> OccupiedCells { get; }
  }
}