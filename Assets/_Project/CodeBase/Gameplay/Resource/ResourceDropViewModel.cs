using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Utility;
using R3;
using UnityEngine;
using Unit = R3.Unit;

namespace _Project.CodeBase.Gameplay.Resource
{
  public class ResourceDropViewModel : IResettablePoolItem<PoolUnit>
  {
    private readonly ICommandBroker _commandBroker;
    private readonly ReactiveProperty<Vector3> _position = new();
    private readonly ReactiveProperty<Vector3> _spawnPoint = new();
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<Unit> _activated = new();

    private IResourceDropReader _resourceDropProxy;

    public int Id => _resourceDropProxy.Id;
    public ReadOnlyReactiveProperty<Vector3> Position => _position;
    public ReadOnlyReactiveProperty<Vector3> SpawnPoint => _spawnPoint;
    public Observable<Unit> Deactivated => _deactivated;

    public Observable<Unit> Activated => _activated;

    public ResourceDropViewModel(ICommandBroker commandBroker)
    {
      _commandBroker = commandBroker;
    }

    public void Setup(IResourceDropReader resourceDropProxy)
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
      UpdateDropSpawnPointCommand command = new UpdateDropSpawnPointCommand(Id, _position.CurrentValue);
      _commandBroker.ExecuteCommand<UpdateDropSpawnPointCommand, bool>(command);
    }
  }
}