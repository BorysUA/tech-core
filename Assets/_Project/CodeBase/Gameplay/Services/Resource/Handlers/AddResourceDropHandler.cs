using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class AddResourceDropHandler : ICommandHandler<AddResourceDropCommand>
  {
    private readonly IProgressService _progressService;

    public AddResourceDropHandler(IProgressService progressService)
    {
      _progressService = progressService;
    }

    public void Execute(AddResourceDropCommand command)
    {
      ResourceDropData resourceDropData =
        new ResourceDropData(GenerateUniqueStringId(), command.ResourceDropType, command.ResourceKind, command.Position,
          command.SpawnPoint, command.Amount);
      ResourceDropProxy resourceDropProxy = new ResourceDropProxy(resourceDropData);
      _progressService.GameStateProxy.ResourceDrops.Add(resourceDropData.Id, resourceDropProxy);
    }
  }
}