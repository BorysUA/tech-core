using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Resource;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using IAssetProvider = _Project.CodeBase.Infrastructure.Services.Interfaces.IAssetProvider;

namespace _Project.CodeBase.Gameplay.Services.Factories
{
  public class GameplayFactory : IGameplayFactory, IGameplayInit
  {
    private const string MeteoritesRootName = "CelestialBodies";
    private const string ResourcesRootName = "Resources";

    private readonly IInstantiator _instantiator;
    private readonly IAssetProvider _assetProvider;
    private readonly IStaticDataProvider _staticDataProvider;

    private readonly MeteoritePool _meteoritesPool = new();
    private readonly ResourceDropPool _resourceDropPool = new();

    private Transform _meteoritesRoot;
    private Transform _resourcesRoot;

    public GameplayFactory(IInstantiator instantiator, IAssetProvider assetProvider,
      IStaticDataProvider staticDataProvider)
    {
      _instantiator = instantiator;
      _assetProvider = assetProvider;
      _staticDataProvider = staticDataProvider;
    }

    public void Initialize()
    {
      CreateRoot();
    }

    public async UniTask<MeteoriteViewModel> CreateMeteorite(MeteoriteType type, Vector3 position)
    {
      if (_meteoritesPool.TryGet(type, out MeteoriteViewModel cachedViewModel))
        return cachedViewModel;

      MeteoriteViewModel viewModel = _instantiator.Instantiate<MeteoriteViewModel>();
      _meteoritesPool.Add(type, viewModel);

      MeteoriteConfig meteoriteConfig = _staticDataProvider.GetMeteoriteConfig(type);
      GameObject meteoritePrefab = await _assetProvider.LoadAssetAsync<GameObject>(meteoriteConfig.PrefabReference);
      MeteoriteView view =
        _instantiator.InstantiatePrefabForComponent<MeteoriteView>(meteoritePrefab, position, Quaternion.identity,
          _meteoritesRoot);
      view.Setup(viewModel);

      return viewModel;
    }

    public async UniTask<ResourceDropViewModel> CreateResourceDrop(ResourceDropType type, Vector3 position)
    {
      if (_resourceDropPool.TryGet(type, out ResourceDropViewModel cachedViewModel))
      {
        cachedViewModel.SetToSpawnPoint(position);
        return cachedViewModel;
      }

      ResourceDropViewModel viewModel = _instantiator.Instantiate<ResourceDropViewModel>();
      _resourceDropPool.Add(type, viewModel);

      ResourceDropConfig resourceDropConfig = _staticDataProvider.GetResourceDropConfig(type);
      GameObject resourceDropPrefab =
        await _assetProvider.LoadAssetAsync<GameObject>(resourceDropConfig.PrefabReference);
      ResourceDropView view =
        _instantiator.InstantiatePrefabForComponent<ResourceDropView>(resourceDropPrefab, position, Quaternion.identity,
          _resourcesRoot);

      view.Setup(viewModel);

      return viewModel;
    }

    private void CreateRoot()
    {
      _meteoritesRoot = new GameObject(MeteoritesRootName).transform;
      _resourcesRoot = new GameObject(ResourcesRootName).transform;
    }
  }
}