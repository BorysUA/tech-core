namespace _Project.CodeBase.Infrastructure.StateMachine.Interfaces
{
  public interface IState : IExitState
  {
    public void Enter();
  }
}