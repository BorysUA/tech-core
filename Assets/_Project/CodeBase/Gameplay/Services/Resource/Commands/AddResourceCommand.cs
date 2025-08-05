using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Results;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct AddResourceCommand : ICommand<ResourceMutationStatus>
  {
    public ResourceKind Kind { get; }
    public int Amount { get; }

    public AddResourceCommand(ResourceKind kind, int amount)
    {
      Kind = kind;
      Amount = amount;
    }
  }
}