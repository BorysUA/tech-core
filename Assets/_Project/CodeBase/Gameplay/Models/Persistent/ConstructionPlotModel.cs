using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent
{
  public class ConstructionPlotModel : IPlotDataReader
  {
    public ConstructionPlotData Source { get; }
    
    public string Id { get; }
    public ConstructionPlotType Type { get; }
    public List<Vector2Int> OccupiedCells { get; }

    public ConstructionPlotModel(ConstructionPlotData constructionPlotData)
    {
      Source = constructionPlotData;

      Id = constructionPlotData.Id;
      Type = constructionPlotData.Type;
      OccupiedCells = constructionPlotData.OccupiedCells;
    }
  }
}