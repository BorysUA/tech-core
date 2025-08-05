using System.Collections.Generic;
using Unity.VisualScripting;

namespace _Project.CodeBase.Gameplay.States.PhaseFlow
{
  public class GameplayPhaseFlow : IGameplayPhaseFlow
  {
    private readonly HashSet<IGamePhaseListener> _listeners = new(ReferenceEqualityComparer.Instance);
    public GameplayPhase Current { get; private set; }

    public GameplayPhaseFlow(List<IGamePhaseListener> initialListeners)
    {
      _listeners.AddRange(initialListeners);
    }

    public void Register(IGamePhaseListener listener)
    {
      _listeners.Add(listener);
      NotifyListener(listener);
    }

    public void Unregister(IGamePhaseListener listener) =>
      _listeners.Remove(listener);

    public void SetPhase(GameplayPhase phase)
    {
      Current = phase;

      foreach (IGamePhaseListener phaseListener in _listeners)
        NotifyListener(phaseListener);
    }

    private void NotifyListener(IGamePhaseListener phaseListener)
    {
      if (Current == GameplayPhase.Started && phaseListener is IGameplayStartedListener startListener)
        startListener.OnGameplayStarted();
      if (Current == GameplayPhase.Paused && phaseListener is IGameplayPausedListener pauseListener)
        pauseListener.OnGameplayPaused();
      if (Current == GameplayPhase.Resumed && phaseListener is IGameplayPausedListener resumeListener)
        resumeListener.OnGameplayResumed();
    }
  }
}