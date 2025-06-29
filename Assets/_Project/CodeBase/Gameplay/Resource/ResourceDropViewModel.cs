using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Utility;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Resource
{
  public class ResourceDropViewModel : IResettablePoolItem<PoolUnit>
  {
    private ResourceDropProxy _resourceDropProxy;
    private readonly ReactiveProperty<Vector3> _position = new();
    private readonly ReactiveProperty<Vector3> _spawnPoint = new();
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<Unit> _activated = new();

    public string Id => _resourceDropProxy.Id;
    public ReadOnlyReactiveProperty<Vector3> Position => _position;
    public ReadOnlyReactiveProperty<Vector3> SpawnPoint => _spawnPoint;
    public Observable<Unit> Deactivated => _deactivated;

    public Observable<Unit> Activated => _activated;

    public void Setup(ResourceDropProxy resourceDropProxy)
    {
      _resourceDropProxy = resourceDropProxy;

      if (VectorUtils.ApproximatelyEqual(_resourceDropProxy.SpawnPoint.CurrentValue, _resourceDropProxy.Position))
        return;

      _position.OnNext(_resourceDropProxy.Position);
    }

    public void SetToSpawnPoint(Vector3 position) =>
      _spawnPoint.OnNext(position);

    public void Activate(PoolUnit param) =>
      _activated.OnNext(Unit.Default);

    public void Deactivate() =>
      _deactivated.OnNext(Unit.Default);

    public void Reset() =>
      _resourceDropProxy = null;

    public void OnMovementCompleted()
    {
      _resourceDropProxy.SpawnPoint.OnNext(_position.CurrentValue);
    }
  }
}