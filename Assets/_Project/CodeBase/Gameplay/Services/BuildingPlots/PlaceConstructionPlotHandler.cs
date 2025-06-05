using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using UnityEngine;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class PlaceConstructionPlotHandler : ICommandHandler<PlaceConstructionPlotCommand>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly IGridOccupancyService _gridOccupancyService;

    public PlaceConstructionPlotHandler(IProgressService progressService, ILogService logService,
      IGridOccupancyService gridOccupancyService)
    {
      _progressService = progressService;
      _logService = logService;
      _gridOccupancyService = gridOccupancyService;
    }

    public void Execute(PlaceConstructionPlotCommand command)
    {
      if (IsCellsOccupied(command.OccupiedCells))
      {
        _logService.LogError(GetType(), " Attempt to place a constructionPlot on already occupied cells");
        return;
      }

      ConstructionPlotData data =
        new ConstructionPlotData(GenerateUniqueStringId(), command.Type, command.OccupiedCells);
      ConstructionPlotDataProxy proxy = new ConstructionPlotDataProxy(data);

      _progressService.GameStateProxy.ConstructionPlotsCollection.Add(proxy);
    }

    private bool IsCellsOccupied(List<Vector2Int> cells)
    {
      foreach (Vector2Int cell in cells)
      {
        if (_gridOccupancyService.TryGetCell(cell, out ICellStatus occupiedCell))
          if (occupiedCell.HasContent(CellContentType.ConstructionPlot))
            return true;
      }

      return false;
    }
  }
}