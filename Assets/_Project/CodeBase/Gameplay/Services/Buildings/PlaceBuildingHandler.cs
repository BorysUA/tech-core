using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using UnityEngine;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class PlaceBuildingHandler : ICommandHandler<PlaceBuildingCommand>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly IGridOccupancyService _gridOccupancyService;

    public PlaceBuildingHandler(IProgressService progressService, ILogService logService,
      IGridOccupancyService gridOccupancyService)
    {
      _logService = logService;
      _gridOccupancyService = gridOccupancyService;
      _progressService = progressService;
    }

    public void Execute(PlaceBuildingCommand command)
    {
      if (IsCellsOccupied(command.OccupiedCells))
      {
        _logService.LogError(GetType(), " Attempt to build a building on already occupied cells");
        return;
      }

      BuildingData buildingData =
        new BuildingData(GenerateUniqueStringId(), command.Type, command.Level, command.OccupiedCells);

      BuildingDataProxy buildingDataProxy = new BuildingDataProxy(buildingData);
      _progressService.GameStateProxy.BuildingsCollection.Add(buildingDataProxy.Id, buildingDataProxy);
    }

    private bool IsCellsOccupied(List<Vector2Int> cells)
    {
      foreach (Vector2Int cell in cells)
      {
        if (_gridOccupancyService.TryGetCell(cell, out var occupiedCell))
          if (occupiedCell.HasContent(CellContentType.Building))
            return true;
      }

      return false;
    }
  }
}