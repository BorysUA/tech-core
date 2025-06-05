using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.InputHandlers;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.ConstructionPlot
{
  public class ConstructionPlotPreview : PlacementPreview
  {
    [SerializeField] private Transform _modelTransform;

    public void Setup(Vector2Int sizeInCells)
    {
      _modelTransform.transform.localScale = new Vector3(sizeInCells.x, sizeInCells.y, 1);
      Activate();
    }
  }
}