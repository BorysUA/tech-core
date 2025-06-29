using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.DataProxy
{
  public class ConstructionPlotDataProxy
  {
    private ConstructionPlotData _origin;
    public string Id { get; }
    public ConstructionPlotType Type { get; }
    public List<Vector2Int> OccupiedCells { get; }
    public ConstructionPlotData Origin => _origin;

    public ConstructionPlotDataProxy(ConstructionPlotData constructionPlotData)
    {
      _origin = constructionPlotData;

      Id = constructionPlotData.Id;
      Type = constructionPlotData.Type;
      OccupiedCells = constructionPlotData.OccupiedCells;
    }
  }
}