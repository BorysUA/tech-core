using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Remote;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Services.DateTimeService;
using _Project.CodeBase.Services.RemoteConfigsService;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public class LiveEventsService : ILiveEventsService, IGameplayInitAsync, IDisposable
  {
    private readonly IRemoteConfigService _remoteConfigService;
    private readonly IDateTimeService _dateTimeService;
    private readonly EventsFactory _eventsFactory;
    private readonly CompositeDisposable _disposable = new();

    private readonly Dictionary<(string, Type), Type> _eventRegistry = new()
    {
      [(RemoteConfigKeys.MeteorShowerEvent, typeof(MeteorShowerEventData))] = typeof(MeteorShowerEvent),
      [(RemoteConfigKeys.ProductionBoostEvent, typeof(ProductionBoostEventData))] = typeof(ProductionBoostEvent),
    };

    private readonly ReactiveProperty<IGameEvent> _currentEvent = new();
    private readonly ReactiveProperty<IGameEvent> _nextEvent = new();

    private List<GameEventBase> _gameEventsTimeline = new();

    public ReadOnlyReactiveProperty<IGameEvent> CurrentEvent => _currentEvent;
    public ReadOnlyReactiveProperty<IGameEvent> NextEvent => _nextEvent;

    public LiveEventsService(IRemoteConfigService remoteConfigService, IDateTimeService dateTimeService,
      EventsFactory eventsFactory)
    {
      _remoteConfigService = remoteConfigService;
      _dateTimeService = dateTimeService;
      _eventsFactory = eventsFactory;
    }

    public async UniTask InitializeAsync()
    {
      await UniTask.WhenAll(_remoteConfigService.WhenReady, _dateTimeService.WhenReady);

      if (!TrySubscribeToServerTime())
        return;

      CreateGameEvents();
    }

    public void Dispose()
    {
      foreach (GameEventBase gameEvent in _gameEventsTimeline)
        gameEvent.Dispose();

      _gameEventsTimeline.Clear();
    }

    private bool TrySubscribeToServerTime()
    {
      if (!_dateTimeService.IsServerTimeAvailable)
        return false;

      _dateTimeService.CurrentTime
        .Subscribe(dateTime =>
        {
          UpdateEventsState(dateTime);
          SelectCurrentAndNextEvents(dateTime);
        })
        .AddTo(_disposable);

      return true;
    }

    private void CreateGameEvents()
    {
      foreach (var eventEntry in _eventRegistry)
      {
        var (remoteConfigKey, remoteConfigType) = eventEntry.Key;
        Type eventType = eventEntry.Value;
        string json = _remoteConfigService.GetValue<string>(remoteConfigKey);

        if (string.IsNullOrEmpty(json))
          continue;

        BaseEventData eventData = JsonConvert.DeserializeObject(json, remoteConfigType) as BaseEventData;

        if (eventData == null || !eventData.Enabled)
          continue;

        GameEventBase newEvent = _eventsFactory.CreateGameEvent(eventType);
        newEvent.Initialize(eventData);
        _gameEventsTimeline.Add(newEvent);
      }

      _gameEventsTimeline = _gameEventsTimeline.OrderBy(gameEvent => gameEvent.EventStartUtc).ToList();
    }

    private void UpdateEventsState(DateTime currentTime)
    {
      foreach (GameEventBase gameEvent in _gameEventsTimeline)
        gameEvent.UpdateActivityStatus(currentTime);
    }

    private void SelectCurrentAndNextEvents(DateTime currentTime)
    {
      GameEventBase newCurrent = null;
      GameEventBase newNext = null;

      for (int i = 0; i < _gameEventsTimeline.Count; i++)
      {
        GameEventBase gameEvent = _gameEventsTimeline[i];

        if (currentTime >= gameEvent.EventEndUtc)
          continue;

        if (gameEvent.IsActive.CurrentValue)
        {
          newCurrent = gameEvent;
          newNext = i + 1 < _gameEventsTimeline.Count ? _gameEventsTimeline[i + 1] : null;
        }
        else
          newNext = gameEvent;

        break;
      }

      _currentEvent.Value = newCurrent;
      _nextEvent.Value = newNext;
    }
  }
}