using System;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Models.Persistent;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using Zenject;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class PlaceConstructionPlotHandler : ICommandHandler<PlaceConstructionPlotCommand, Unit>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;
    private readonly IGridOccupancyQuery _gridOccupancyQuery;
    private readonly SignalBus _signalBus;
    private readonly IResourceMutator _resourceMutator;
    private readonly IStaticDataProvider _staticDataProvider;

    public PlaceConstructionPlotHandler(IProgressWriter progressService, ILogService logService,
      IGridOccupancyQuery gridOccupancyQuery, SignalBus signalBus, IResourceMutator resourceMutator,
      IStaticDataProvider staticDataProvider)
    {
      _progressService = progressService;
      _logService = logService;
      _gridOccupancyQuery = gridOccupancyQuery;
      _signalBus = signalBus;
      _resourceMutator = resourceMutator;
      _staticDataProvider = staticDataProvider;
    }

    public Unit Execute(in PlaceConstructionPlotCommand command)
    {
      ConstructionPlotConfig config = _staticDataProvider.GetConstructionPlotConfig(command.Type);

      if (!_gridOccupancyQuery.DoesCellsMatchFilter(command.OccupiedCells, config.PlacementFilter))
      {
        _logService.LogError(GetType(),
          $"Attempt to place constructionPlot {config.Type} on invalid cells: [{string.Join(", ", command.OccupiedCells)}]");
        return Unit.Default;
      }

      Span<ResourceAmountData> toSpend = stackalloc ResourceAmountData[1] { config.Price };
      ResourceMutationResult result = _resourceMutator.TrySpend(toSpend);

      if (result.IsFailure)
        return Unit.Default;

      int plotId = GenerateUniqueIntId();

      ConstructionPlotData data =
        new ConstructionPlotData(plotId, command.Type, command.OccupiedCells);
      ConstructionPlotModel proxy = new ConstructionPlotModel(data);

      _progressService.GameStateModel.WriteOnlyPlots.Add(proxy);
      _signalBus.Fire(new ConstructionPlotPlaced(plotId));

      return Unit.Default;
    }
  }
}