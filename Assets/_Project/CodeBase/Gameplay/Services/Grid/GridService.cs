using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public class GridService : IGridService
  {
    private const int CellSize = 1;

    private Vector2Int _gameFieldBottomLeft = new(-5, -5);
    private Vector2Int _gameFieldTopRight = new(5, 5);

    public Vector3 GetSnappedPosition(Vector3 worldPosition)
    {
      float snappedX = Mathf.Round(worldPosition.x - 0.5f);
      float snappedZ = Mathf.Round(worldPosition.z - 0.5f);

      return new Vector3(snappedX, 0, snappedZ);
    }

    public Vector3 GetSnappedPosition(Vector3 worldPosition, Vector2Int size)
    {
      float halfW = size.x / 2f;
      float halfH = size.y / 2f;

      Vector2 correctedPos = new Vector2(
        worldPosition.x - halfW,
        worldPosition.z - halfH
      );

      float snappedX = Mathf.Round(correctedPos.x);
      float snappedY = Mathf.Round(correctedPos.y);

      return new Vector3(snappedX + halfW, 0, snappedY + halfH);
    }

    public Vector3 GetWorldPivot(IEnumerable<Vector2Int> cells)
    {
      float sumX = 0;
      float sumY = 0;
      int count = 0;

      foreach (Vector2Int cell in cells)
      {
        sumX += cell.x;
        sumY += cell.y;
        count++;
      }

      float centerX = sumX / count + (float)CellSize / 2;
      float centerY = sumY / count + (float)CellSize / 2;

      return new Vector3(centerX, 0, centerY);
    }

    public Vector3 GetWorldPivot(Vector2Int cell)
    {
      float centerX = cell.x + (float)CellSize / 2;
      float centerY = cell.y + (float)CellSize / 2;

      return new Vector3(centerX, 0, centerY);
    }

    public Vector2Int GetCell(Vector3 snappedPosition)
    {
      return new Vector2Int(Mathf.RoundToInt(snappedPosition.x), Mathf.RoundToInt(snappedPosition.z));
    }

    public List<Vector2Int> GetCells(Vector3 snappedPosition, Vector2Int size)
    {
      List<Vector2Int> occupiedCells = new List<Vector2Int>(size.x * size.y);

      float halfW = size.x / 2f;
      float halfH = size.y / 2f;

      float centerX = snappedPosition.x;
      float centerY = snappedPosition.z;

      float bottomLeftX = centerX - halfW;
      float bottomLeftY = centerY - halfH;

      for (int x = 0; x < size.x; x++)
      {
        for (int y = 0; y < size.y; y++)
        {
          int cellX = Mathf.RoundToInt(bottomLeftX + x);
          int cellY = Mathf.RoundToInt(bottomLeftY + y);
          occupiedCells.Add(new Vector2Int(cellX, cellY));
        }
      }

      return occupiedCells;
    }

    public Vector2Int GetRandomCell()
    {
      int x = Random.Range(_gameFieldBottomLeft.x, _gameFieldTopRight.x);
      int y = Random.Range(_gameFieldBottomLeft.y, _gameFieldTopRight.y);
      return new Vector2Int(x, y);
    }
  }
}