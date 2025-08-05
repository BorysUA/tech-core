using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class SpendResourcesHandler : ICommandHandler<SpendResourcesCommand, ResourceMutationStatus>
  {
    private readonly SignalBus _signalBus;
    private readonly IResourceMutator _resourceMutator;

    public SpendResourcesHandler(SignalBus signalBus, IResourceMutator resourceMutator)
    {
      _signalBus = signalBus;
      _resourceMutator = resourceMutator;
    }

    public ResourceMutationStatus Execute(in SpendResourcesCommand command)
    {
      ResourceMutationResult resourceMutationResult = _resourceMutator.TrySpend(command.Resources);

      if (resourceMutationResult.IsSuccess)
      {
        foreach (ResourceAmountData resourceToSpend in command.Resources)
          _signalBus.Fire(new ResourcesSpent(command.Sink, resourceToSpend.Kind, resourceToSpend.Amount));
      }

      return resourceMutationResult.Code;
    }
  }
}