using System;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Services.AnalyticsService.Constants;
using Zenject;

namespace _Project.CodeBase.Services.AnalyticsService.Trackers
{
  public class BuildingPurchaseTracker : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly IAnalyticsService _analyticsService;

    public BuildingPurchaseTracker(SignalBus signalBus, IAnalyticsService analyticsService)
    {
      _signalBus = signalBus;
      _analyticsService = analyticsService;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<BuildingPurchaseRequested>(OnBuildingPurchaseRequested);
      _signalBus.Subscribe<ConstructionPlotPurchaseRequested>(OnPlotPurchaseRequested);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<BuildingPurchaseRequested>(OnBuildingPurchaseRequested);
      _signalBus.Unsubscribe<ConstructionPlotPurchaseRequested>(OnPlotPurchaseRequested);
    }

    private void OnBuildingPurchaseRequested(BuildingPurchaseRequested request) =>
      _analyticsService.LogEvent(EventNames.BuildingPurchaseRequested, (ParameterKeys.Type, request.Type));

    private void OnPlotPurchaseRequested(ConstructionPlotPurchaseRequested request) =>
      _analyticsService.LogEvent(EventNames.ConstructionPlotPurchaseRequested, (ParameterKeys.Type, request.Type));
  }
}
