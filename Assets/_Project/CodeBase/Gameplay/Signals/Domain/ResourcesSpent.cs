using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Resource;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ResourcesSpent
  {
    public readonly ResourceSink Sink;
    public readonly ResourceKind Kind;
    public readonly int Amount;

    public ResourcesSpent(ResourceSink sink, ResourceKind kind, int amount)
    {
      Sink = sink;
      Kind = kind;
      Amount = amount;
    }
  }
}