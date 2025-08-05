using _Project.CodeBase.Gameplay.Constants;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IResourceDropReader
  {
    public int Id { get; }
    public ResourceKind ResourceKind { get; }
    public ResourceDropType ResourceDropType { get; }
    public Vector3 Position { get; }
    public ReadOnlyReactiveProperty<Vector3> SpawnPoint { get; }
    public int Amount { get; }
  }
}