using System;
using System.Globalization;
using _Project.CodeBase.Gameplay.Signals.System;
using _Project.CodeBase.Services.AnalyticsService.Constants;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Services.AnalyticsService.Trackers
{
  public class SessionMetadataTracker : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly IAnalyticsService _analyticsService;

    private float _sessionStartTime;

    public SessionMetadataTracker(IAnalyticsService analyticsService, SignalBus signalBus)
    {
      _analyticsService = analyticsService;
      _signalBus = signalBus;
    }

    public void Initialize()
    {
      _signalBus.Subscribe<GameSessionPaused>(OnPause);
      _signalBus.Subscribe<GameSessionEnded>(OnQuit);
      _signalBus.Subscribe<GameSessionStarted>(OnStart);
    }

    public void Dispose()
    {
      _signalBus.Unsubscribe<GameSessionStarted>(OnStart);
      _signalBus.Unsubscribe<GameSessionPaused>(OnPause);
      _signalBus.Unsubscribe<GameSessionEnded>(OnQuit);
    }

    private void OnStart()
    {
      _analyticsService.SetUserProperty(UserProperties.Language, Application.systemLanguage.ToString());
      _analyticsService.SetUserProperty(UserProperties.Region, RegionInfo.CurrentRegion.Name);
      _analyticsService.SetUserProperty(UserProperties.AppVersion, Application.version);
      _analyticsService.LogEvent(EventNames.GameStarted);
    }

    private void OnPause(GameSessionPaused isPaused)
    {
      if (isPaused.Status)
        LogSessionMetadata();
      else
        _sessionStartTime = Time.realtimeSinceStartup;
    }

    private void OnQuit() =>
      LogSessionMetadata();

    private void LogSessionMetadata()
    {
      float duration = Time.realtimeSinceStartup - _sessionStartTime;
      _analyticsService.LogEvent(EventNames.GamePaused, parameters: (ParameterKeys.ElapsedTime, duration));
    }
  }
}