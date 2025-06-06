using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Meteorite.VFX;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services
{
  public class VFXFactory : IVFXFactory, IOnLoadInitializable
  {
    private const string VFXRoot = "VFXRoot";

    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;

    private readonly ObjectPool<ExplosionEffect> _explosionsPool = new();
    private readonly ObjectPool<TrailEffect> _trailPool = new();

    private Transform _vfxRoot;

    public VFXFactory(IStaticDataProvider staticDataProvider, IAssetProvider assetProvider, IInstantiator instantiator)
    {
      _staticDataProvider = staticDataProvider;
      _assetProvider = assetProvider;
      _instantiator = instantiator;
    }

    public void Initialize()
    {
      CreateRoot();
    }

    public async UniTask<IExplosionEffect> CreateExplosionEffect(MeteoriteType meteoriteType, Vector3 position)
    {
      MeteoriteVFX meteoriteVFXs = _staticDataProvider.GetMeteoriteVFXs(meteoriteType);

      ExplosionEffect explosion = await GetOrCreate(meteoriteVFXs.ExplosionPrefab, _explosionsPool);
      explosion.Setup(position, meteoriteVFXs.ExplosionScale, meteoriteVFXs.CameraShakePreset);

      return explosion;
    }

    public async UniTask<ITrailEffect> CreateTrailEffect(MeteoriteType meteoriteType, Transform parent)
    {
      MeteoriteVFX meteoriteVFXs = _staticDataProvider.GetMeteoriteVFXs(meteoriteType);

      TrailEffect trail = await GetOrCreate(meteoriteVFXs.TrailPrefab, _trailPool);
      trail.Initialize(_vfxRoot);
      trail.Setup(parent);

      return trail;
    }

    private async UniTask<T> GetOrCreate<T>(AssetReference address, ObjectPool<T> pool) where T : class, IPoolItem
    {
      if (pool.TryGet(out T item))
        return item;

      GameObject model = await _assetProvider.LoadAssetAsync<GameObject>(address);
      T instance = _instantiator.InstantiatePrefab(model, _vfxRoot).GetComponent<T>();
      pool.Add(instance);
      return instance;
    }

    private void CreateRoot()
    {
      _vfxRoot = new GameObject(VFXRoot).transform;
    }
  }
}