using _Project.CodeBase.Infrastructure.StateMachine;

namespace _Project.CodeBase.Gameplay.States
{
  public interface IGameplayInit
  {
    public InitPhase InitPhase { get; }
    public void Initialize();
  }
}