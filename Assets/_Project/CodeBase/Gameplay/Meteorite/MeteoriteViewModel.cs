using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Building.Modules.EnergyShield;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Building.VFX.Module;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Markers;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Utility;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using static UnityEngine.Random;

namespace _Project.CodeBase.Gameplay.Meteorite
{
  public class MeteoriteViewModel : IResettablePoolItem<PoolUnit>
  {
    private const float Tolerance = 0.1f;

    private readonly IResourceService _resourceService;
    private readonly IBuildingService _buildingService;

    private readonly ReactiveProperty<Vector3> _position = new();
    private readonly ReactiveProperty<Quaternion> _rotation = new();
    private readonly ReactiveProperty<Vector3> _direction = new();
    private readonly Subject<Unit> _activated = new();
    private readonly Subject<Unit> _deactivated = new();
    private readonly Subject<Unit> _initialized = new();
    private readonly Subject<Unit> _exploded = new();

    private MeteoriteConfig _meteoriteConfig;
    private Vector3 _target;
    private Vector3 _randomRotationAxis;

    public MeteoriteType Type => _meteoriteConfig.Type;
    public ReadOnlyReactiveProperty<Vector3> Position => _position;
    public ReadOnlyReactiveProperty<Quaternion> Rotation => _rotation;
    public ReadOnlyReactiveProperty<Vector3> Direction => _direction;
    public Observable<Unit> Initialized => _initialized;
    public Observable<Unit> Activated => _activated;
    public Observable<Unit> Deactivated => _deactivated;

    public Observable<Unit> Exploded => _exploded;

    public MeteoriteViewModel(IResourceService resourceService, IBuildingService buildingService)
    {
      _resourceService = resourceService;
      _buildingService = buildingService;
    }

    public void Setup(Vector3 target, Vector3 startPosition, MeteoriteConfig meteoriteConfig)
    {
      _meteoriteConfig = meteoriteConfig;
      _target = target;
      _position.OnNext(startPosition);
      _rotation.OnNext(Quaternion.identity);
      _randomRotationAxis = onUnitSphere;

      _initialized.OnNext(Unit.Default);
    }

    public void Activate(PoolUnit param)
    {
      _activated.OnNext(Unit.Default);
    }

    public void Deactivate() =>
      _deactivated.OnNext(Unit.Default);

    public void Reset()
    {
      _position.OnNext(Vector3.zero);
      _target = Vector3.zero;
      _meteoriteConfig = null;
    }

    public void Update(float deltaTime)
    {
      MoveToTarget(deltaTime);
      Rotate(deltaTime);
    }

    public async UniTask HandleMeteorCollision(Collider other)
    {
      if (other.TryGetComponent(out Ground ground))
      {
        DropResources();
        Explode();
      }

      if (other.TryGetComponent(out EnergyShieldEffect shieldEffect))
      {
        if (await shieldEffect.DamageInterceptor.TryInterceptDamage(_meteoriteConfig.Damage))
          Explode();
      }

      if (other.TryGetComponent(out BuildingView building))
      {
        IBuildingViewInteractor buildingInteractor = building.BuildingViewInteractor;

        if (buildingInteractor.TryGetPublicModuleContract(out IDamageable damageable))
          damageable.TakeDamage(_meteoriteConfig.Damage);

        Explode();
      }
    }

    private void MoveToTarget(float deltaTime)
    {
      Vector3 toTarget = _target - _position.CurrentValue;

      if (toTarget.sqrMagnitude < Tolerance * Tolerance)
        return;

      Vector3 direction = toTarget.normalized;

      float remainingDistance = toTarget.magnitude;
      float moveDistance = Mathf.Min(_meteoriteConfig.MoveSpeed * deltaTime, remainingDistance);

      _position.Value += direction * moveDistance;
      _direction.Value = direction;
    }

    private void Rotate(float deltaTime)
    {
      _rotation.Value *= Quaternion.AngleAxis(_meteoriteConfig.RotationSpeed * deltaTime, _randomRotationAxis);
    }

    private void Explode()
    {
      _exploded.OnNext(Unit.Default);
    }

    private void DropResources()
    {
      foreach (ResourceDropConfig resourceDrop in _meteoriteConfig.ResourceDrops)
      {
        int amount = Range(resourceDrop.ResourceRange.Amount.Min, resourceDrop.ResourceRange.Amount.Max);

        Vector3 resourceDropPosition = _position.CurrentValue.ToXZ() +
                                       VectorUtils.GetRandomXZDirection() *
                                       _meteoriteConfig.ExplosionArea.magnitude / 2;

        _resourceService.AddResourceDrop(resourceDrop.Type, resourceDrop.ResourceRange.Resource.Kind,
          _position.CurrentValue,
          resourceDropPosition, amount);
      }
    }
  }
}