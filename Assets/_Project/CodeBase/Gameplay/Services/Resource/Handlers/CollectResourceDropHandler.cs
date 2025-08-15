using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Services.LogService;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class CollectResourceDropHandler : ICommandHandler<CollectResourceDropCommand, CollectResourceDropResult>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;
    private readonly IResourceMutator _resourceMutator;

    public CollectResourceDropHandler(IProgressWriter progressService,
      ILogService logService, SignalBus signalBus, IResourceMutator resourceMutator)
    {
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
      _resourceMutator = resourceMutator;
    }

    public CollectResourceDropResult Execute(in CollectResourceDropCommand command)
    {
      if (!_progressService.GameStateModel.WriteOnlyResourceDrops.TryGetValue(command.Id,
            out IResourceDropWriter dropWriter))
      {
        _logService.LogError(GetType(),
          $"Resource drop with ID '{command.Id}' not found in '{nameof(_progressService.GameStateModel.WriteOnlyResourceDrops)}'.");

        return new CollectResourceDropResult(false);
      }

      Span<ResourceAmountData> toAdd = stackalloc ResourceAmountData[1]
        { new ResourceAmountData(dropWriter.ResourceKind, dropWriter.Amount) };

      Span<ResourceAmountData> resultBuffer = stackalloc ResourceAmountData[1];

      ResourceMutationResult resourceMutationResult = _resourceMutator.TryAddFlexible(toAdd, resultBuffer);

      if (resourceMutationResult.IsSuccess)
      {
        _signalBus.Fire(new ResourcesGained(ResourceSource.Drop, resultBuffer[0].Kind, resultBuffer[0].Amount,
          command.Id));

        _progressService.GameStateModel.WriteOnlyResourceDrops.Remove(command.Id);

        return new CollectResourceDropResult(
          true,
          dropWriter.ResourceKind,
          resultBuffer[0].Amount,
          dropWriter.Position);
      }

      return new CollectResourceDropResult(false);
    }
  }
}