using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Signals.Domain;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class SpendResourceHandler : ICommandHandler<SpendResourceCommand, ResourceMutationStatus>
  {
    private readonly SignalBus _signalBus;
    private readonly IResourceMutator _resourceMutator;

    public SpendResourceHandler(SignalBus signalBus, IResourceMutator resourceMutator)
    {
      _signalBus = signalBus;
      _resourceMutator = resourceMutator;
    }

    public ResourceMutationStatus Execute(in SpendResourceCommand command)
    {
      Span<ResourceAmountData> toSpend = stackalloc ResourceAmountData[1]
        { new ResourceAmountData(command.Kind, command.Amount) };

      ResourceMutationResult resourceMutationResult = _resourceMutator.TrySpend(toSpend);

      if (resourceMutationResult.IsSuccess)
        _signalBus.Fire(new ResourcesSpent(command.Sink, command.Kind, command.Amount));

      return resourceMutationResult.Code;
    }
  }
}