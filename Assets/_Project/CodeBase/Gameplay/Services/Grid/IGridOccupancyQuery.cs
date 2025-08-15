using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public interface IGridOccupancyQuery
  {
    public CellStatus GetCellOrEmpty(Vector2Int position);  
    CellContentType GetCellContentMask(Vector2Int position);
    bool DoesCellMatchFilter(Vector2Int cellPosition, PlacementFilter filter);
    bool DoesCellsMatchFilter(IEnumerable<Vector2Int> cellsPosition, PlacementFilter filter);
  }
}