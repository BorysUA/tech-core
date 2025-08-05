using System;
using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Services.Command
{
  public class CommandBroker : ICommandBroker
  {
    private readonly Dictionary<Type, object> _handlers = new();

    public void Register<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler)
      where TCommand : struct, ICommand<TResult> =>
      _handlers.Add(typeof(TCommand), handler);

    public TResult ExecuteCommand<TCommand, TResult>(in TCommand command)
      where TCommand : struct, ICommand<TResult>
    {
      if (_handlers.TryGetValue(typeof(TCommand), out object value) &&
          value is ICommandHandler<TCommand, TResult> handler)
        return handler.Execute(command);

      throw new InvalidOperationException(
        $"Handler for {typeof(TCommand).Name} with result {typeof(TResult).Name} is not registered.");
    }

    public Unit ExecuteCommand<TCommand>(in TCommand command)
      where TCommand : struct, ICommand<Unit> =>
      ExecuteCommand<TCommand, Unit>(command);
  }
}