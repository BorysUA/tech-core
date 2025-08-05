namespace _Project.CodeBase.Gameplay.Services.Command
{
  public interface ICommandHandler<TCommand, out TResult>
  {
    TResult Execute(in TCommand command);
  }
}