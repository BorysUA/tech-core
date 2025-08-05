using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Signals.Domain;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class AddResourceHandler : ICommandHandler<AddResourceCommand, ResourceMutationStatus>
  {
    private readonly SignalBus _signalBus;
    private readonly IResourceMutator _resourceMutator;

    public AddResourceHandler(SignalBus signalBus, IResourceMutator resourceMutator)
    {
      _signalBus = signalBus;
      _resourceMutator = resourceMutator;
    }

    public ResourceMutationStatus Execute(in AddResourceCommand command)
    {
      Span<ResourceAmountData> toAdd = stackalloc ResourceAmountData[1]
        { new ResourceAmountData(command.Kind, command.Amount) };
      Span<ResourceAmountData> resultBuffer = stackalloc ResourceAmountData[1];

      ResourceMutationResult result = _resourceMutator.TryAddFlexible(toAdd, resultBuffer);

      if (result.IsSuccess)
        _signalBus.Fire(new ResourcesGained(ResourceSource.Direct, resultBuffer[0].Kind, resultBuffer[0].Amount));

      return result.Code;
    }
  }
}