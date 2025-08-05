using System;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.Progress
{
  [Serializable]
  public class ResourceDropData
  {
    public int Id;
    public ResourceKind ResourceKind;
    public ResourceDropType ResourceDropType;
    public Vector3 SpawnPosition;
    public Vector3 Position;
    public int Amount;

    public ResourceDropData(int id, ResourceDropType resourceDropType, ResourceKind resourceKind, Vector3 position,
      Vector3 spawnPosition, int amount)
    {
      ResourceKind = resourceKind;
      ResourceDropType = resourceDropType;
      Position = position;
      Amount = amount;
      SpawnPosition = spawnPosition;
      Id = id;
    }
  }
}