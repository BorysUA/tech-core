using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using ObservableCollections;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class ConstructionPlotService : IConstructionPlotService, IGameplayInit
  {
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ICommandBroker _commandBroker;
    private readonly IResourceService _resourceService;
    private readonly ObservableList<ConstructionPlotInfo> _availablePlots = new();

    public IObservableCollection<ConstructionPlotInfo> AvailablePlots => _availablePlots;

    public InitPhase InitPhase => InitPhase.Preparation;

    public ConstructionPlotService(IStaticDataProvider staticDataProvider,
      ICommandBroker commandBroker, IResourceService resourceService)
    {
      _staticDataProvider = staticDataProvider;
      _commandBroker = commandBroker;
      _resourceService = resourceService;
    }

    public void Initialize()
    {
      foreach (ConstructionPlotConfig config in _staticDataProvider.GetAllBuildingPlots())
        _availablePlots.Add(new ConstructionPlotInfo(config.Type, config.SizeInCells));
    }

    public void PlaceConstructionPlot(ConstructionPlotType type, List<Vector2Int> placementResultCells)
    {
      ConstructionPlotConfig config = _staticDataProvider.GetConstructionPlotConfig(type);

      if (_resourceService.TrySpend(config.Price.Kind, ResourceSink.Construction, config.Price.Amount))
      {
        PlaceConstructionPlotCommand command = new(type, placementResultCells);
        _commandBroker.ExecuteCommand(command);
      }
    }
  }
}