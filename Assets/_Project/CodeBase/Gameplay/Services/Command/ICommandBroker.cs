namespace _Project.CodeBase.Gameplay.Services.Command
{
  public interface ICommandBroker
  {
    TResult ExecuteCommand<TCommand, TResult>(in TCommand command)
      where TCommand : struct, ICommand<TResult>;

    Unit ExecuteCommand<TCommand>(in TCommand command)
      where TCommand : struct, ICommand<Unit>;

    void Register<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler)
      where TCommand : struct, ICommand<TResult>;
  }
}