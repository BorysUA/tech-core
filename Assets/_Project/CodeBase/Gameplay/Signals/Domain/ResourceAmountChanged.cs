using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Signals.Domain
{
  public readonly struct ResourceAmountChanged
  {
    public readonly ResourceKind Kind;
    public readonly int Delta;
    public readonly int NewTotal;

    public ResourceAmountChanged(ResourceKind kind, int delta, int newTotal)
    {
      Delta = delta;
      NewTotal = newTotal;
      Kind = kind;
    }
  }
}