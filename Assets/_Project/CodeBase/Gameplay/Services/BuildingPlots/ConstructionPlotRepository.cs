using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using _Project.CodeBase.Gameplay.Models;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class ConstructionPlotRepository : IGameplayInit, IDisposable
  {
    private readonly IConstructionPlotFactory _constructionPlotFactory;
    private readonly IProgressReader _progressService;
    private readonly Dictionary<int, ConstructionPlotViewModel> _constructionPlots = new();
    private readonly CompositeDisposable _subscriptions = new();

    public ConstructionPlotRepository(IConstructionPlotFactory constructionPlotFactory, IProgressReader progressService)
    {
      _constructionPlotFactory = constructionPlotFactory;
      _progressService = progressService;
    }

    public InitPhase InitPhase => InitPhase.Creation;

    public void Initialize()
    {
      foreach (IPlotDataReader plotDataReader in _progressService.GameStateModel.ReadOnlyPlots.Values)
        CreateView(plotDataReader).Forget();

      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveAdd()
        .Subscribe(addEvent => CreateView(addEvent.Value).Forget())
        .AddTo(_subscriptions);

      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveRemove()
        .Subscribe(removeValue => DestroyView(removeValue.Value))
        .AddTo(_subscriptions);
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
      _subscriptions?.Dispose();
  }
}