using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Buildings.VFX;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings
{
  public class BuildingView : MonoBehaviour
  {
    [SerializeField] private GameObject _selectionBorder;
    [SerializeField] private SimpleEffect[] _buildingEffects;
    [SerializeField] private ModuleEffect[] _modulesEffects;

    private BuildingViewModel _viewModel;

    public IBuildingViewInteractor BuildingViewInteractor => _viewModel;

    public void Setup(BuildingViewModel viewModel)
    {
      _viewModel = viewModel;

      viewModel.IsInitialized
        .Subscribe(_ => OnInitialized())
        .AddTo(this);
    }

    private void OnInitialized()
    {
      foreach (ModuleEffect moduleEffect in _modulesEffects)
      {
        if (_viewModel.TryGetModuleUnsafe(moduleEffect.ModuleType, out BuildingModule module))
          moduleEffect.BindModule(module);
      }

      _viewModel.Selected
        .Subscribe(_ => ShowSelectionBorder())
        .AddTo(this);

      _viewModel.Unselected
        .Subscribe(_ => HideSelectionBorder())
        .AddTo(this);

      _viewModel.Destroyed
        .Subscribe(_ => Destroy(gameObject))
        .AddTo(this);

      _viewModel.BuildingOperational
        .Subscribe(OnOperationalStatusChanged)
        .AddTo(this);
    }

    private void OnOperationalStatusChanged(bool isOperational)
    {
      if (isOperational)
        PlayVFX();
      else
        StopVFX();
    }

    private void PlayVFX()
    {
      foreach (SimpleEffect effect in _buildingEffects)
        effect.Play();
    }

    private void StopVFX()
    {
      foreach (SimpleEffect effect in _buildingEffects)
        effect.Stop();
    }

    private void ShowSelectionBorder() =>
      _selectionBorder.SetActive(true);

    private void HideSelectionBorder() =>
      _selectionBorder.SetActive(false);
  }
}