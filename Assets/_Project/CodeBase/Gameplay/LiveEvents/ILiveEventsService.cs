using R3;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public interface ILiveEventsService
  {
    ReadOnlyReactiveProperty<IGameEvent> CurrentEvent { get; }
    ReadOnlyReactiveProperty<IGameEvent> NextEvent { get; }
  }
}