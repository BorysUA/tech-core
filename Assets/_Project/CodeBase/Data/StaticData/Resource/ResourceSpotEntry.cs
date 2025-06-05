using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [Serializable]
  public struct ResourceSpotEntry
  {
    public ResourceKind Kind;
    public List<Vector2Int> OccupiedCells;

    public ResourceSpotEntry(ResourceKind kind, List<Vector2Int> occupiedCells)
    {
      Kind = kind;
      OccupiedCells = occupiedCells;
    }
  }
}