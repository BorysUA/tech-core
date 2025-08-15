using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class ConstructionPlotService : IConstructionPlotService, IDisposable, IGameplayInit
  {
    private readonly IConstructionPlotFactory _constructionPlotFactory;
    private readonly IProgressReader _progressService;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ICommandBroker _commandBroker;
    private readonly IResourceService _resourceService;
    private readonly CompositeDisposable _disposable = new();

    private readonly Dictionary<int, ConstructionPlotViewModel> _constructionPlots = new();
    private readonly ObservableList<ConstructionPlotInfo> _availablePlots = new();

    public IObservableCollection<ConstructionPlotInfo> AvailablePlots => _availablePlots;

    public ConstructionPlotService(IProgressReader progressService, IStaticDataProvider staticDataProvider,
      ICommandBroker commandBroker, IConstructionPlotFactory constructionPlotFactory,
      IResourceService resourceService)
    {
      _progressService = progressService;
      _staticDataProvider = staticDataProvider;
      _commandBroker = commandBroker;
      _constructionPlotFactory = constructionPlotFactory;
      _resourceService = resourceService;
    }

    public void Initialize()
    {
      foreach (ConstructionPlotConfig config in _staticDataProvider.GetAllBuildingPlots())
        _availablePlots.Add(new ConstructionPlotInfo(config.Type, config.SizeInCells));

      foreach (IPlotDataReader plotDataReader in _progressService.GameStateModel.ReadOnlyPlots.Values) 
        CreateView(plotDataReader).Forget();

      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveAdd()
        .Subscribe(addEvent => CreateView(addEvent.Value).Forget())
        .AddTo(_disposable);

      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveRemove()
        .Subscribe(removeValue => DestroyView(removeValue.Value))
        .AddTo(_disposable);
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

    private async UniTaskVoid CreateView(IPlotDataReader data)
    {
      Vector3 worldPivot = GridUtils.GetWorldPivot(data.OccupiedCells);
      ConstructionPlotViewModel
        viewModel = await _constructionPlotFactory.CreateConstructionPlot(data.Type, worldPivot);

      _constructionPlots.Add(data.Id, viewModel);
    }

    private void DestroyView(IPlotDataReader data)
    {
      if (_constructionPlots.Remove(data.Id, out ConstructionPlotViewModel viewModel))
        viewModel.Destroy();
    }

    public void Dispose() =>
      _disposable?.Dispose();
  }
}