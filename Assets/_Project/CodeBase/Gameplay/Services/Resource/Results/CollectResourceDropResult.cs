using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Results
{
  public struct CollectResourceDropResult : ICommandResult
  {
    public bool IsSuccessful { get; }
    public ResourceKind ResourceKind { get; }
    public int Amount { get; }
    public Vector3 Position { get; }

    public CollectResourceDropResult(bool isSuccessful, ResourceKind resourceKind = ResourceKind.None, int amount = 0,
      Vector3 position = default)
    {
      IsSuccessful = isSuccessful;
      ResourceKind = resourceKind;
      Amount = amount;
      Position = position;
    }
  }
}