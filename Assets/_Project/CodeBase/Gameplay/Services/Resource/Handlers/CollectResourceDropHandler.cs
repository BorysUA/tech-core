using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class CollectResourceDropHandler : ICommandHandler<CollectResourceDropCommand, CollectResourceDropResult>
  {
    private readonly IProgressService _progressService;
    private readonly ICommandBroker _commandBroker;
    private readonly ILogService _logService;

    public CollectResourceDropHandler(ICommandBroker commandBroker, IProgressService progressService,
      ILogService logService)
    {
      _commandBroker = commandBroker;
      _progressService = progressService;
      _logService = logService;
    }

    public CollectResourceDropResult Execute(CollectResourceDropCommand command)
    {
      if (!_progressService.GameStateProxy.ResourceDrops.TryGetValue(command.Id,
            out ResourceDropProxy resourceDropProxy))
      {
        LogResourceDropNotFound(command.Id);
        return new CollectResourceDropResult(false);
      }

      AddResourceResult addResult = TryAddResource(resourceDropProxy);
      if (addResult.IsSuccessful)
      {
        RemoveResourceDrop(command.Id);
        return new CollectResourceDropResult(
          true,
          resourceDropProxy.ResourceKind,
          addResult.Amount,
          resourceDropProxy.Position);
      }

      return new CollectResourceDropResult(false);
    }

    private AddResourceResult TryAddResource(ResourceDropProxy resourceDropProxy)
    {
      var addCommand = new AddResourceCommand(resourceDropProxy.ResourceKind, resourceDropProxy.Amount);
      return _commandBroker.ExecuteCommand<AddResourceCommand, AddResourceResult>(addCommand);
    }

    private void RemoveResourceDrop(string resourceDropId)
    {
      _progressService.GameStateProxy.ResourceDrops.Remove(resourceDropId);
    }

    private void LogResourceDropNotFound(string id)
    {
      _logService.LogError(GetType(),
        $"Resource drop with ID '{id}' not found in '{nameof(_progressService.GameStateProxy.ResourceDrops)}'.");
    }
  }
}