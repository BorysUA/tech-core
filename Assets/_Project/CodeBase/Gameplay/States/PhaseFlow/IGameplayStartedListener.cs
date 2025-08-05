namespace _Project.CodeBase.Gameplay.States.PhaseFlow
{
  public interface IGameplayStartedListener : IGamePhaseListener
  {
    void OnGameplayStarted();
  }
}