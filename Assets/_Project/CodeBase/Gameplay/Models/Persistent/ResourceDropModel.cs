using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent
{
  public class ResourceDropModel : IResourceDropWriter
  {
    public ResourceDropData Source { get; }
    
    public int Id { get; }
    public ResourceKind ResourceKind { get; }
    public ResourceDropType ResourceDropType { get; }
    public Vector3 Position { get; }
    ReadOnlyReactiveProperty<Vector3> IResourceDropReader.SpawnPoint => SpawnPoint;
    public ReactiveProperty<Vector3> SpawnPoint { get; }
    public int Amount { get; }

    public ResourceDropModel(ResourceDropData resourceDropData)
    {
      Source = resourceDropData;

      Id = resourceDropData.Id;
      ResourceKind = resourceDropData.ResourceKind;
      ResourceDropType = resourceDropData.ResourceDropType;
      Position = resourceDropData.Position;
      SpawnPoint = new ReactiveProperty<Vector3>(resourceDropData.SpawnPosition);
      Amount = resourceDropData.Amount;

      SpawnPoint.Subscribe(value => resourceDropData.SpawnPosition = value);
    }
  }
}