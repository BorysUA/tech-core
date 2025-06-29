using System;
using _Project.CodeBase.Gameplay.LiveEvents;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD.GameEvent
{
  public class GameEventViewData
  {
    public GameEventType Type { get; init; }
    public bool IsActive { get; init; }
    public ReadOnlyReactiveProperty<TimeSpan> Countdown { get; init; }
  }
}