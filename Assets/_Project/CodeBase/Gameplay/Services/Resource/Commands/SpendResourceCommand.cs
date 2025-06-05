using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Resource.Commands
{
  public readonly struct SpendResourceCommand : ICommand
  {
    public ResourceKind Kind { get; }
    public int Amount { get; }

    public SpendResourceCommand(ResourceKind kind, int amount)
    {
      Kind = kind;
      Amount = amount;
    }
  }
}