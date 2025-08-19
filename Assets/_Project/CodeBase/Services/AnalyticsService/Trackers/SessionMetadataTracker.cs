using System;
using System.Globalization;
using _Project.CodeBase.Infrastructure.Signals;
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

    public void Initialize() =>
      _signalBus.Subscribe<AppLifecycleChanged>(OnAppLifecycleChanged);

    public void Dispose() =>
      _signalBus.Unsubscribe<AppLifecycleChanged>(OnAppLifecycleChanged);

    private void OnAppLifecycleChanged(AppLifecycleChanged lifecycleStatus)
    {
      switch (lifecycleStatus.Current)
      {
        case AppLifecycleChanged.Phase.Started:
          OnStart();
          break;
        case AppLifecycleChanged.Phase.Paused:
          OnPause();
          break;
        case AppLifecycleChanged.Phase.Resumed:
          OnResume();
          break;
        case AppLifecycleChanged.Phase.Quited:
          OnQuit();
          break;
      }
    }

    private void OnStart()
    {
      _analyticsService.SetUserProperty(UserProperties.Language, Application.systemLanguage.ToString());
      _analyticsService.SetUserProperty(UserProperties.Region, RegionInfo.CurrentRegion.Name);
      _analyticsService.SetUserProperty(UserProperties.AppVersion, Application.version);
      _analyticsService.LogEvent(EventNames.GameStarted);
    }

    private void OnPause() =>
      LogSessionMetadata();

    private void OnResume() =>
      _sessionStartTime = Time.realtimeSinceStartup;

    private void OnQuit() =>
      LogSessionMetadata();

    private void LogSessionMetadata()
    {
      float duration = Time.realtimeSinceStartup - _sessionStartTime;
      _analyticsService.LogEvent(EventNames.GamePaused,
        parameters: EventParameter.Create(ParameterKeys.ElapsedTime, duration));
    }
  }
}