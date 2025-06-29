using System;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Gameplay.Services;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD.GameEvent
{
  public class GameEventViewModel : IDisposable
  {
    private readonly ILiveEventsService _liveEventsService;
    private readonly CompositeDisposable _disposable = new();
    private readonly ReactiveProperty<GameEventViewData> _focusedEvent = new();

    public ReadOnlyReactiveProperty<GameEventViewData> FocusedEvent => _focusedEvent;

    public GameEventViewModel(ILiveEventsService liveEventsService)
    {
      _liveEventsService = liveEventsService;
    }

    public void Initialize()
    {
      _liveEventsService.CurrentEvent
        .CombineLatest(_liveEventsService.NextEvent, (currentEvent, nextEvent) => currentEvent ?? nextEvent)
        .Subscribe(UpdateFocusedEvent)
        .AddTo(_disposable);
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }

    private void UpdateFocusedEvent(IGameEvent gameEvent)
    {
      _focusedEvent.OnNext(gameEvent is null
        ? new GameEventViewData
        {
          Type = GameEventType.None
        }
        : new GameEventViewData
        {
          Type = gameEvent.Type,
          IsActive = gameEvent.IsActive.CurrentValue,
          Countdown = gameEvent.TimeUntilStart.CombineLatest(gameEvent.TimeUntilCompletion,
              (timeStart, timeEnd) => gameEvent.IsActive.CurrentValue ? timeEnd : timeStart)
            .ToReadOnlyReactiveProperty()
        });
    }
  }
}