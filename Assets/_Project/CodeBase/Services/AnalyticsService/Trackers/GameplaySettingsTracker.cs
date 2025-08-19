using System;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Services.AnalyticsService.Constants;
using Zenject;

namespace _Project.CodeBase.Services.AnalyticsService.Trackers
{
  public class GameplaySettingsTracker : IInitializable, IDisposable
  {
    private readonly SignalBus _signalBus;
    private readonly IAnalyticsService _analyticsService;

    public GameplaySettingsTracker(SignalBus signalBus, IAnalyticsService analyticsService)
    {
      _signalBus = signalBus;
      _analyticsService = analyticsService;
    }

    public void Initialize() =>
      _signalBus.Subscribe<GameplaySceneLoadRequested>(OnLoadGameplay);

    public void Dispose() =>
      _signalBus.Unsubscribe<GameplaySceneLoadRequested>(OnLoadGameplay);

    private void OnLoadGameplay(GameplaySceneLoadRequested request)
    {
      _analyticsService.SetUserProperty(UserProperties.Difficulty, request.GameplaySettings.GameDifficulty.ToString());
      _analyticsService.LogEvent(EventNames.DifficultySelected,
        EventParameter.Create(ParameterKeys.Difficulty, request.GameplaySettings.GameDifficulty));
    }
  }
}