using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public struct IncreaseResourceCapacityCommand : ICommand
  {
    public ResourceKind ResourceKind { get; }
    public int CapacityDelta { get; }

    public IncreaseResourceCapacityCommand(ResourceKind resourceKind, int capacityDelta)
    {
      ResourceKind = resourceKind;
      CapacityDelta = capacityDelta;
    }
  }
}