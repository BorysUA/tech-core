using System;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public readonly struct ResourceStorage
  {
    public int Amount { get; }
    public int Capacity { get; }

    public ResourceStorage(int amount, int capacity)
    {
      Amount = amount;
      Capacity = capacity;
    }
  }
}