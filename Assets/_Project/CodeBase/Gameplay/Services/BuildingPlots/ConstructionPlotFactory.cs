using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public class ConstructionPlotFactory : IConstructionPlotFactory, IGameplayInit
  {
    private const string RootName = "ConstructionPlotsRoot";

    private readonly IAssetProvider _assetProvider;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IInstantiator _instantiator;
    private Transform _constructionPlotsRoot;

    public ConstructionPlotFactory(IAssetProvider assetProvider, IStaticDataProvider staticDataProvider,
      IInstantiator instantiator)
    {
      _assetProvider = assetProvider;
      _staticDataProvider = staticDataProvider;
      _instantiator = instantiator;
    }

    public void Initialize() => 
      CreateRoot();

    public async UniTask<ConstructionPlotViewModel> CreateConstructionPlot(ConstructionPlotType type,
      Vector3 worldPivot)
    {
      ConstructionPlotConfig config = _staticDataProvider.GetConstructionPlotConfig(type);
      GameObject prefab = await _assetProvider.LoadAssetAsync<GameObject>(config.PrefabReference);
      ConstructionPlotView view = _instantiator.InstantiatePrefabForComponent<ConstructionPlotView>(prefab, worldPivot,
        Quaternion.identity, _constructionPlotsRoot);

      ConstructionPlotViewModel viewModel = _instantiator.Instantiate<ConstructionPlotViewModel>();
      view.Setup(viewModel);

      return viewModel;
    }

    public async UniTask<ConstructionPlotPreview> CreateConstructionPlotPreview()
    {
      GameObject previewPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.PlotPreview);
      return Object.Instantiate(previewPrefab, _constructionPlotsRoot).GetComponent<ConstructionPlotPreview>();
    }

    private void CreateRoot()
    {
      _constructionPlotsRoot = new GameObject(RootName).transform;
    }
  }
}