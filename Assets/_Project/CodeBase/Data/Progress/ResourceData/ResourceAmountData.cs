using System;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Data.Progress.ResourceData
{
  [Serializable]
  public struct ResourceAmountData
  {
    public ResourceKind Kind;
    public int Amount;

    public ResourceAmountData(ResourceKind kind, int amount)
    {
      Kind = kind;
      Amount = amount;
    }
  }
}