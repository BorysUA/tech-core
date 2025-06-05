using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class ConstructionPlotService : IConstructionPlotService, IDisposable
  {
    private readonly IConstructionPlotFactory _constructionPlotFactory;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ICommandBroker _commandBroker;
    private readonly IGridService _gridService;
    private readonly IResourceService _resourceService;
    private readonly CompositeDisposable _disposable = new();

    private readonly Dictionary<string, ConstructionPlotViewModel> _constructionPlots = new();
    private readonly ObservableList<ConstructionPlotType> _availableBuildingPlots = new();

    public IObservableCollection<ConstructionPlotType> AvailableConstructionPlots => _availableBuildingPlots;

    public ConstructionPlotService(IProgressService progressService, IStaticDataProvider staticDataProvider,
      ICommandBroker commandBroker, IConstructionPlotFactory constructionPlotFactory, IGridService gridService,
      IResourceService resourceService)
    {
      _staticDataProvider = staticDataProvider;
      _commandBroker = commandBroker;
      _constructionPlotFactory = constructionPlotFactory;
      _gridService = gridService;
      _resourceService = resourceService;

      foreach (ConstructionPlotConfig buildingPlotConfig in staticDataProvider.GetAllBuildingPlots())
        _availableBuildingPlots.Add(buildingPlotConfig.Type);

      foreach (ConstructionPlotDataProxy data in progressService.GameStateProxy.ConstructionPlotsCollection)
        CreateView(data);

      progressService.GameStateProxy.ConstructionPlotsCollection
        .ObserveAdd()
        .Subscribe(addEvent => CreateView(addEvent.Value))
        .AddTo(_disposable);

      progressService.GameStateProxy.ConstructionPlotsCollection
        .ObserveRemove()
        .Subscribe(removeValue => DestroyView(removeValue.Value))
        .AddTo(_disposable);
    }

    public void PlaceConstructionPlot(ConstructionPlotType type, List<Vector2Int> placementResultCells)
    {
      ConstructionPlotConfig config = _staticDataProvider.GetConstructionPlotConfig(type);

      if (_resourceService.TrySpend(config.Price.Resource.Kind, config.Price.Amount))
      {
        PlaceConstructionPlotCommand command = new(type, placementResultCells);
        _commandBroker.ExecuteCommand(command);
      }
    }

    private async void CreateView(ConstructionPlotDataProxy data)
    {
      Vector3 worldPivot = _gridService.GetWorldPivot(data.OccupiedCells);
      ConstructionPlotViewModel
        viewModel = await _constructionPlotFactory.CreateConstructionPlot(data.Type, worldPivot);

      _constructionPlots.Add(data.Id, viewModel);
    }

    private void DestroyView(ConstructionPlotDataProxy data)
    {
      if (_constructionPlots.Remove(data.Id, out ConstructionPlotViewModel viewModel))
        viewModel.Destroy();
    }

    public void Dispose() =>
      _disposable?.Dispose();
  }
}