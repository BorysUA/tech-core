using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Data
{
  public class ResourceDropProxy
  {
    public string Id { get; }
    public ResourceKind ResourceKind { get; }
    public ResourceDropType ResourceDropType { get; }
    public Vector3 Position { get; }
    public ReactiveProperty<Vector3> SpawnPoint { get; }
    public int Amount { get; }
    public ResourceDropData Origin { get; }

    public ResourceDropProxy(ResourceDropData resourceDropData)
    {
      Id = resourceDropData.Id;
      ResourceKind = resourceDropData.ResourceKind;
      ResourceDropType = resourceDropData.ResourceDropType;
      Position = resourceDropData.Position;
      SpawnPoint = new ReactiveProperty<Vector3>(resourceDropData.SpawnPosition);
      Origin = resourceDropData;
      Amount = resourceDropData.Amount;

      SpawnPoint.Subscribe(value => resourceDropData.SpawnPosition = value);
    }
  }
}