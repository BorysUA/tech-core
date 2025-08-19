using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Signals;
using _Project.CodeBase.Services.AnalyticsService.Constants;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Services.AnalyticsService.Trackers
{
  public class ResourceAccumulationTracker : IInitializable, IDisposable
  {
    private const float MinLogInterval = 300f;

    private readonly SignalBus _signalBus;
    private readonly IAnalyticsService _analyticsService;

    private readonly Dictionary<ResourceKind, int> _gained = new();
    private readonly Dictionary<ResourceKind, int> _spent = new();

    private float _lastLogTime;

    public ResourceAccumulationTracker(SignalBus signalBus, IAnalyticsService analyticsService)
    {
      _signalBus = signalBus;
      _analyticsService = analyticsService;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<ResourcesGained>(OnResourcesGained);
      _signalBus.Subscribe<ResourcesSpent>(OnResourcesSpent);
      _signalBus.Subscribe<AppLifecycleChanged>(OnLifecycleChanged);
    }

    private void OnLifecycleChanged(AppLifecycleChanged lifecycleStatus)
    {
      switch (lifecycleStatus.Current)
      {
        case AppLifecycleChanged.Phase.Paused:
          OnPause();
          break;
        case AppLifecycleChanged.Phase.Quited:
          OnQuit();
          break;
      }
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<ResourcesGained>(OnResourcesGained);
      _signalBus.Unsubscribe<ResourcesSpent>(OnResourcesSpent);
      _signalBus.Unsubscribe<AppLifecycleChanged>(OnLifecycleChanged);
    }

    private void OnPause()
    {
      if (Time.realtimeSinceStartup - _lastLogTime > MinLogInterval)
        SendSnapshot();
    }

    private void OnQuit() =>
      SendSnapshot();

    private void SendSnapshot()
    {
      foreach ((ResourceKind kind, int amount) in _gained)
        _analyticsService.LogEvent(EventNames.ResourceGained,
          EventParameter.Create(ParameterKeys.Type, kind),
          EventParameter.Create(ParameterKeys.Amount, amount));

      foreach ((ResourceKind kind, int amount) in _spent)
        _analyticsService.LogEvent(EventNames.ResourceSpent,
          EventParameter.Create(ParameterKeys.Type, kind),
          EventParameter.Create(ParameterKeys.Amount, amount));

      _gained.Clear();
      _spent.Clear();

      _lastLogTime = Time.realtimeSinceStartup;
    }

    private void OnResourcesGained(ResourcesGained gained) =>
      _gained[gained.Kind] = _gained.GetValueOrDefault(gained.Kind) + gained.Amount;

    private void OnResourcesSpent(ResourcesSpent spent) =>
      _spent[spent.Kind] = _spent.GetValueOrDefault(spent.Kind) + spent.Amount;
  }
}