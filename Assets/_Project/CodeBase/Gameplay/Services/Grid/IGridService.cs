using System.Collections.Generic;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public interface IGridService
  {
    Vector3 GetSnappedPosition(Vector3 worldPosition, Vector2Int size);
    List<Vector2Int> GetCells(Vector3 snappedPosition, Vector2Int size);
    Vector3 GetWorldPivot(IEnumerable<Vector2Int> cells);
    Vector2Int GetRandomCell();
    Vector3 GetWorldPivot(Vector2Int cell);
    Vector2Int GetCell(Vector3 snappedPosition);
    Vector3 GetSnappedPosition(Vector3 worldPosition);
  }
}