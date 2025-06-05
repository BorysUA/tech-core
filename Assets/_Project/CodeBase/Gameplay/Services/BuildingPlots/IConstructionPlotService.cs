using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public interface IConstructionPlotService
  {
    IObservableCollection<ConstructionPlotType> AvailableConstructionPlots { get; }
    void PlaceConstructionPlot(ConstructionPlotType type, List<Vector2Int> placementResultCells);
  }
}