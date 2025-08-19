using System;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Services.AnalyticsService.Constants;
using Zenject;

namespace _Project.CodeBase.Services.AnalyticsService.Trackers
{
  public class BuildingLifecycleTracker : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly IAnalyticsService _analyticsService;

    public BuildingLifecycleTracker(SignalBus signalBus, IAnalyticsService analyticsService)
    {
      _signalBus = signalBus;
      _analyticsService = analyticsService;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<BuildingPlaced>(OnBuildingPlaced);
      _signalBus.Subscribe<ConstructionPlotPlaced>(OnPlotPlaced);
      _signalBus.Subscribe<BuildingDestroyed>(OnBuildingDestroyed);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<BuildingPlaced>(OnBuildingPlaced);
      _signalBus.Unsubscribe<ConstructionPlotPlaced>(OnPlotPlaced);
      _signalBus.Unsubscribe<BuildingDestroyed>(OnBuildingDestroyed);
    }

    private void OnBuildingPlaced(BuildingPlaced data)
    {
      _analyticsService.LogEvent(EventNames.BuildingPlaced, EventParameter.Create(ParameterKeys.Id, data.BuildingId));
    }

    private void OnPlotPlaced(ConstructionPlotPlaced data) =>
      _analyticsService.LogEvent(EventNames.ConstructionPlotPlaced,
        EventParameter.Create(ParameterKeys.Type, data.PlotId));

    private void OnBuildingDestroyed(BuildingDestroyed data) =>
      _analyticsService.LogEvent(EventNames.BuildingDestroyed,
        EventParameter.Create(ParameterKeys.Id, data.Id),
        EventParameter.Create(ParameterKeys.Type, data.Type),
        EventParameter.Create(ParameterKeys.Level, data.Level));
  }
}