using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.Conditions;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using IAssetProvider = _Project.CodeBase.Infrastructure.Services.Interfaces.IAssetProvider;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class BuildingFactory : IBuildingFactory, IGameplayInit
  {
    private const string RootName = "BuildingsRoot";

    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ContractToModuleRegistry _contractToModuleRegistry;
    private readonly ILogService _logService;

    private Transform _buildingsRoot;
    private BuildingPreview _buildingPreview;

    public BuildingFactory(IAssetProvider assetProvider, IInstantiator instantiator,
      IStaticDataProvider staticDataProvider, ContractToModuleRegistry contractToModuleRegistry, ILogService logService)
    {
      _assetProvider = assetProvider;
      _instantiator = instantiator;
      _staticDataProvider = staticDataProvider;
      _contractToModuleRegistry = contractToModuleRegistry;
      _logService = logService;
    }

    public void Initialize() =>
      CreateRoot();

    public async UniTask<BuildingViewModel> CreateBuilding(BuildingType buildingType, Vector3 position)
    {
      BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(buildingType);
      GameObject buildingPrefab = await _assetProvider.LoadAssetAsync<GameObject>(buildingConfig.PrefabReference);
      BuildingView view = _instantiator.InstantiatePrefabForComponent<BuildingView>(buildingPrefab, position,
        Quaternion.identity, _buildingsRoot);

      BuildingViewModel viewModel = _instantiator.Instantiate<BuildingViewModel>();

      List<BuildingModule> modules = new List<BuildingModule>();

      foreach (BuildingModuleConfig moduleConfig in buildingConfig.BuildingsModules)
      {
        BuildingModule module =
          moduleConfig.CreateModule(InstantiateModule, InstantiateCondition, buildingConfig, _logService);

        modules.Add(module);
      }

      viewModel.Setup(modules);
      view.Setup(viewModel);

      return viewModel;
    }

    public async UniTask<BuildingPreview> CreateBuildingPreview(BuildingType buildingType)
    {
      if (_buildingPreview is null)
      {
        GameObject previewPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.BuildingPreview);
        _buildingPreview = Object.Instantiate(previewPrefab, _buildingsRoot).GetComponent<BuildingPreview>();
      }

      AssetReferenceT<Mesh> meshReference = _staticDataProvider.GetBuildingConfig(buildingType).MeshReference;
      Mesh buildingPreviewMesh = await _assetProvider.LoadAssetAsync<Mesh>(meshReference);

      _buildingPreview.Setup(buildingPreviewMesh);
      _buildingPreview.Activate();

      return _buildingPreview;
    }

    private BuildingModule InstantiateModule(Type moduleType)
    {
      _contractToModuleRegistry.RegisterModuleContracts(moduleType);
      return _instantiator.Instantiate(moduleType) as BuildingModule;
    }

    private OperationalCondition InstantiateCondition(Type conditionType) =>
      _instantiator.Instantiate(conditionType) as OperationalCondition;

    private void CreateRoot() =>
      _buildingsRoot = new GameObject(RootName).transform;
  }
}