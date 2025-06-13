using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ResourceDropCollected
  {
    public readonly ResourceKind ResourceKind;
    public readonly int Amount;

    public ResourceDropCollected(ResourceKind resourceKind, int amount)
    {
      ResourceKind = resourceKind;
      Amount = amount;
    }
  }
}