namespace _Project.CodeBase.Infrastructure.StateMachine.Interfaces
{
  public interface IPayloadState<in T> : IState
  {
    void Enter(T payload);
  }
}