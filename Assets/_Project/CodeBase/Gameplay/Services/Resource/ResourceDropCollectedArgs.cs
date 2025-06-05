using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public struct ResourceDropCollectedArgs 
  {
    public ResourceKind ResourceKind { get; }
    public int Amount { get; }
    public Vector3 Position { get; }

    public ResourceDropCollectedArgs(ResourceKind resourceKind, int amount, Vector3 position)
    {
      ResourceKind = resourceKind;
      Amount = amount;
      Position = position;
    }
  }
}