using System.Collections.Generic;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings
{
  public struct PlacementResult
  {
    public readonly List<Vector2Int> Cells;
    public readonly bool IsConfirmed;

    public PlacementResult(List<Vector2Int> cells, bool isConfirmed)
    {
      Cells = cells;
      IsConfirmed = isConfirmed;
    }
  }
}