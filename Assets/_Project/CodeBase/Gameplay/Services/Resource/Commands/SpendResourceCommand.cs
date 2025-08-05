using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct SpendResourceCommand : ICommand<ResourceMutationStatus>
  {
    public ResourceKind Kind { get; }
    public ResourceSink Sink { get; }
    public int Amount { get; }

    public SpendResourceCommand(ResourceKind kind, int amount, ResourceSink sink)
    {
      Kind = kind;
      Amount = amount;
      Sink = sink;
    }
  }
}