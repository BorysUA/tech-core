namespace _Project.CodeBase.Gameplay.Services.Command
{
  public interface ICommandBroker
  {
    void Register<TCommand>(ICommandHandler<TCommand> commandHandler) where TCommand : ICommand;
    void Register<TCommand, TResult>(ICommandHandler<TCommand, TResult> commandHandler) where TCommand : ICommand;
    void ExecuteCommand<TCommand>(in TCommand command) where TCommand : struct, ICommand;
    TResult ExecuteCommand<TCommand, TResult>(in TCommand command) where TCommand : struct, ICommand;
  }
}