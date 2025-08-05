using R3;

namespace _Project.CodeBase.Gameplay.Services.Timers
{
  public interface ISessionTimer
  {
    ReadOnlyReactiveProperty<float> SessionPlaytime { get; }
  }
}