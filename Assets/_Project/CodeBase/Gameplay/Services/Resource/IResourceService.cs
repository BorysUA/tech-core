using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public interface IResourceService
  {
    bool TrySpend(ResourceKind kind, int amount);
    void AddResource(ResourceKind kind, int amount);
    ReadOnlyReactiveProperty<int> ObserveResource(ResourceKind kind);
    void AddResourceDrop(ResourceDropType resourceDropType, ResourceKind resourceKind, Vector3 spawnPoint, Vector3 position,
      int amount);
    void CollectDrop(string id);
    ReadOnlyReactiveProperty<ResourceDropCollectedArgs> ResourceDropCollected { get; }
    bool TrySpend(params ResourceAmountData[] resources);
    bool TryReserve(ResourceKind kind, int amount, out ReservationToken token);
    bool IncreaseCapacity(ResourceKind kind, int amount);
    bool DecreaseCapacity(ResourceKind kind, int amount);
  }
}