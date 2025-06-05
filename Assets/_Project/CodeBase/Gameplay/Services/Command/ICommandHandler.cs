namespace _Project.CodeBase.Gameplay.Services.Command
{
  public interface ICommandHandler<in TCommand> where TCommand : ICommand
  {
    public void Execute(TCommand command);
  }

  public interface ICommandHandler<in TCommand, out TResult> where TCommand : ICommand
  {
    public TResult Execute(TCommand command);
  }
}