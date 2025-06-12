using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Gameplay.Signals.System;
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
    private readonly Dictionary<ResourceKind, int> _dropsLooted = new();

    private float _lastLogTime;

    public ResourceAccumulationTracker(SignalBus signalBus, IAnalyticsService analyticsService)
    {
      _signalBus = signalBus;
      _analyticsService = analyticsService;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<ResourceAmountChanged>(OnResourceChanged);
      _signalBus.Subscribe<ResourceDropCollected>(OnDropCollected);
      _signalBus.Subscribe<GameSessionPaused>(OnPause);
      _signalBus.Subscribe<GameSessionEnded>(OnQuit);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<ResourceAmountChanged>(OnResourceChanged);
      _signalBus.Unsubscribe<ResourceDropCollected>(OnDropCollected);
      _signalBus.Unsubscribe<GameSessionPaused>(OnPause);
      _signalBus.Unsubscribe<GameSessionEnded>(OnQuit);
    }

    private void OnPause(GameSessionPaused isPaused)
    {
      if (isPaused.Status && Time.realtimeSinceStartup - _lastLogTime > MinLogInterval)
        SendSnapshot();
    }

    private void OnQuit() =>
      SendSnapshot();

    private void SendSnapshot()
    {
      foreach ((ResourceKind kind, int amount) in _gained)
        _analyticsService.LogEvent(EventNames.ResourceGained, (ParameterKeys.Type, kind), (ParameterKeys.Amount, amount));

      foreach ((ResourceKind kind, int amount) in _spent)
        _analyticsService.LogEvent(EventNames.ResourceSpent, (ParameterKeys.Type, kind), (ParameterKeys.Amount, amount));

      foreach ((ResourceKind kind, int amount) in _dropsLooted)
        _analyticsService.LogEvent(EventNames.ResourceDropLooted, (ParameterKeys.Type, kind), (ParameterKeys.Amount, amount));

      _dropsLooted.Clear();
      _gained.Clear();
      _spent.Clear();

      _lastLogTime = Time.realtimeSinceStartup;
    }

    private void OnResourceChanged(ResourceAmountChanged resource)
    {
      var targetStorage = resource.Delta > 0 ? _gained : _spent;
      int delta = Mathf.Abs(resource.Delta);

      targetStorage[resource.Kind] = targetStorage.GetValueOrDefault(resource.Kind) + delta;
    }

    private void OnDropCollected(ResourceDropCollected drop) =>
      _dropsLooted[drop.ResourceKind] = _dropsLooted.GetValueOrDefault(drop.ResourceKind) + drop.Amount;
  }
}