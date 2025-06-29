using System;
using System.Globalization;
using _Project.CodeBase.Data.Remote;
using R3;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public abstract class GameEventBase : IDisposable, IGameEvent
  {
    private readonly ReactiveProperty<bool> _isActive = new();
    private readonly ReactiveProperty<TimeSpan> _timeUntilStart = new(TimeSpan.Zero);
    private readonly ReactiveProperty<TimeSpan> _timeUntilCompletion = new(TimeSpan.Zero);

    public abstract GameEventType Type { get; }
    public DateTime EventStartUtc { get; private set; }
    public DateTime EventEndUtc { get; private set; }

    public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
    public ReadOnlyReactiveProperty<TimeSpan> TimeUntilStart => _timeUntilStart;
    public ReadOnlyReactiveProperty<TimeSpan> TimeUntilCompletion => _timeUntilCompletion;

    public virtual void Initialize(BaseEventData eventData)
    {
      EventStartUtc = DateTime.ParseExact(eventData.StartUtc, "O", CultureInfo.InvariantCulture,
        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

      EventEndUtc = DateTime.ParseExact(eventData.EndUtc, "O", CultureInfo.InvariantCulture,
        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

      _isActive
        .DistinctUntilChanged()
        .Skip(1)
        .Subscribe(active =>
        {
          if (active)
            OnActivate();
          else
            OnDeactivate();
        });
    }

    public void UpdateActivityStatus(DateTime currentTime)
    {
      UpdateTimeLeft(currentTime);
      _isActive.Value = currentTime >= EventStartUtc && currentTime < EventEndUtc;
    }

    public virtual void Dispose()
    {
    }

    protected abstract void OnActivate();

    protected abstract void OnDeactivate();

    private void UpdateTimeLeft(DateTime dateTimeNow)
    {
      _timeUntilStart.Value = ClampToNonNegative(EventStartUtc - dateTimeNow);
      _timeUntilCompletion.Value = ClampToNonNegative(EventEndUtc - dateTimeNow);
    }

    private TimeSpan ClampToNonNegative(TimeSpan expectedTime)
    {
      int compareResult = TimeSpan.Compare(expectedTime, TimeSpan.Zero);
      return compareResult == -1 ? TimeSpan.Zero : expectedTime;
    }
  }
}