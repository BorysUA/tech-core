namespace _Project.CodeBase.Gameplay.States.PhaseFlow
{
  public interface IGameplayPhaseFlow
  {
    public GameplayPhase Current { get; }
    public void Register(IGamePhaseListener listener);
    void Unregister(IGamePhaseListener listener);
  }
}