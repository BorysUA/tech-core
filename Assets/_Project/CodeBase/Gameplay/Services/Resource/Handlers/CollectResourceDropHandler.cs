using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class CollectResourceDropHandler : ICommandHandler<CollectResourceDropCommand, CollectResourceDropResult>
  {
    private readonly IProgressService _progressService;
    private readonly ICommandBroker _commandBroker;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;

    public CollectResourceDropHandler(ICommandBroker commandBroker, IProgressService progressService,
      ILogService logService, SignalBus signalBus)
    {
      _commandBroker = commandBroker;
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
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
        _signalBus.Fire(new ResourceDropCollected(resourceDropProxy.ResourceKind, addResult.Amount));

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