using _Project.CodeBase.Gameplay.Data;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public interface IGridOccupancyService
  {
    bool TryGetCell(Vector2Int position, out ICellStatus cellData);
  }
}