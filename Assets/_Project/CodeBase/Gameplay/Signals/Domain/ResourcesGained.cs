using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Resource;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ResourcesGained
  {
    public readonly ResourceSource Source;
    public readonly ResourceKind Kind;
    public readonly int Amount;
    public readonly int SourceId;

    public ResourcesGained(ResourceSource source, ResourceKind kind, int amount, int sourceId = -1)
    {
      Source = source;
      SourceId = sourceId;
      Kind = kind;
      Amount = amount;
    }
  }
}