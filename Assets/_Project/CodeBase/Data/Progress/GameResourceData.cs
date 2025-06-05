using System;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Data.Progress
{
  [Serializable]
  public class GameResourceData
  {
    public ResourceKind Kind;
    public int Amount;
    public int Capacity;

    public GameResourceData(ResourceKind kind, int amount, int capacity)
    {
      Kind = kind;
      Amount = amount;
      Capacity = capacity;
    }
  }
}