namespace _Project.CodeBase.Gameplay.States.PhaseFlow
{
  public interface IGameplayPausedListener : IGamePhaseListener
  {
    void OnGameplayPaused();
    void OnGameplayResumed();
  }
}