using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Meteorite.VFX;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Factories
{
  public class VFXFactory : IVFXFactory, IGameplayInit
  {
    private const string VFXRoot = "VFXRoot";

    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;

    private readonly ObjectPool<ExplosionEffect> _explosionsPool = new();
    private readonly ObjectPool<TrailEffect, Transform> _trailPool = new();

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

      ExplosionEffect explosion = await GetOrCreate(meteoriteVFXs.ExplosionPrefab, PoolUnit.Default, _explosionsPool);
      explosion.Setup(position, meteoriteVFXs.ExplosionScale, meteoriteVFXs.CameraShakePreset);

      return explosion;
    }

    public async UniTask<ITrailEffect> CreateTrailEffect(MeteoriteType meteoriteType, Transform parent)
    {
      MeteoriteVFX meteoriteVFXs = _staticDataProvider.GetMeteoriteVFXs(meteoriteType);

      TrailEffect trail = await GetOrCreate(meteoriteVFXs.TrailPrefab, parent, _trailPool);
      trail.Initialize(_vfxRoot);

      return trail;
    }

    private async UniTask<TItem> GetOrCreate<TItem, TParam>(AssetReference address, TParam param,
      ObjectPool<TItem, TParam> pool)
      where TItem : class, IPoolItem<TParam>
    {
      if (pool.TryGet(param, out TItem item))
        return item;

      GameObject model = await _assetProvider.LoadAssetAsync<GameObject>(address);
      TItem instance = _instantiator.InstantiatePrefab(model, _vfxRoot).GetComponent<TItem>();
      pool.Add(instance);
      return instance;
    }

    private void CreateRoot()
    {
      _vfxRoot = new GameObject(VFXRoot).transform;
    }
  }
}