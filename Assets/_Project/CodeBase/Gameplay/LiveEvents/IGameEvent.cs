using System;
using R3;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public interface IGameEvent
  {
    GameEventType Type { get; }
    ReadOnlyReactiveProperty<bool> IsActive { get; }
    ReadOnlyReactiveProperty<TimeSpan> TimeUntilStart { get; }
    ReadOnlyReactiveProperty<TimeSpan> TimeUntilCompletion { get; }
  }
}