using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Zenject;
using UnityEngine;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class PlaceBuildingHandler : ICommandHandler<PlaceBuildingCommand, Unit>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;
    private readonly IGridOccupancyService _gridOccupancyService;
    private readonly SignalBus _signalBus;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IResourceMutator _resourceMutator;

    public PlaceBuildingHandler(IProgressWriter progressService, ILogService logService,
      IGridOccupancyService gridOccupancyService, SignalBus signalBus, IStaticDataProvider staticDataProvider,
      IResourceMutator resourceMutator)
    {
      _logService = logService;
      _gridOccupancyService = gridOccupancyService;
      _progressService = progressService;
      _signalBus = signalBus;
      _staticDataProvider = staticDataProvider;
      _resourceMutator = resourceMutator;
    }

    public Unit Execute(in PlaceBuildingCommand command)
    {
      BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(command.Type);

      if (!_gridOccupancyService.DoesCellsMatchFilter(command.OccupiedCells, buildingConfig.PlacementFilter))
      {
        _logService.LogError(GetType(),
          $"Attempt to place building {buildingConfig.Type} on invalid cells: [{string.Join(", ", command.OccupiedCells)}]");
        return Unit.Default;
      }

      Span<ResourceAmountData> toSpend = stackalloc ResourceAmountData[1] { buildingConfig.Price };
      ResourceMutationResult resourceMutationResult = _resourceMutator.TrySpend(toSpend);

      if (resourceMutationResult.IsFailure)
        return Unit.Default;

      int buildingId = GenerateUniqueIntId();
      Dictionary<Type, IModuleData> modulesData = new();

      foreach (BuildingModuleConfig moduleConfig in buildingConfig.BuildingsModules)
      {
        if (moduleConfig is IModuleProgressFactory moduleProgressFactory)
        {
          var (moduleType, data) = moduleProgressFactory.CreateInitialData();
          modulesData[moduleType] = data;
        }
      }

      Vector2Int[] cellsSnapshot = command.OccupiedCells.ToArray();
      BuildingData buildingData = new BuildingData(buildingId, command.Type, command.Level, modulesData,
        cellsSnapshot);

      BuildingModel buildingModel = new BuildingModel(buildingData);
      _progressService.GameStateModel.WriteOnlyBuildings.Add(buildingModel.Id, buildingModel);

      _signalBus.Fire(new BuildingPlaced(buildingId));

      return Unit.Default;
    }
  }
}