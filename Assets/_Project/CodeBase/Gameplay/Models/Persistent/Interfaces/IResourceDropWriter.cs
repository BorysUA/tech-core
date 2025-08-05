using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IResourceDropWriter : IResourceDropReader
  {
    public new ReactiveProperty<Vector3> SpawnPoint { get; }
  }
}