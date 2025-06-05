using System;
using System.Collections.Generic;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Command
{
  public class CommandBroker : ICommandBroker
  {
    private readonly Dictionary<Type, object> _handlers = new();
    private readonly ILogService _logService;

    public CommandBroker(ILogService logService)
    {
      _logService = logService;
    }

    public void Register<TCommand>(ICommandHandler<TCommand> commandHandler) where TCommand : ICommand =>
      _handlers.Add(typeof(TCommand), commandHandler);

    public void Register<TCommand, TResult>(ICommandHandler<TCommand, TResult> commandHandler)
      where TCommand : ICommand =>
      _handlers.Add(typeof(TCommand), commandHandler);

    public void ExecuteCommand<TCommand>(in TCommand command) where TCommand : struct, ICommand
    {
      if (_handlers.TryGetValue(typeof(TCommand), out object value))
      {
        ICommandHandler<TCommand> commandHandler = value as ICommandHandler<TCommand>;
        commandHandler?.Execute(command);
      }
    }

    public TResult ExecuteCommand<TCommand, TResult>(in TCommand command) where TCommand : struct, ICommand
    {
      if (_handlers.TryGetValue(typeof(TCommand), out object value))
        if (value is ICommandHandler<TCommand, TResult> commandHandler)
          return commandHandler.Execute(command);

      _logService.LogError(GetType(), $"Unexpected command {typeof(TCommand)}");
      return default;
    }
  }
}