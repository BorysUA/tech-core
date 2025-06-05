namespace _Project.CodeBase.Infrastructure.StateMachine.Interfaces
{
  public interface IPayloadState<in T> : IExitState
  {
    void Enter(T payload);
  }
}