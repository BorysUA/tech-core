using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building
{
  public class BuildingView : MonoBehaviour
  {
    public string Id => _viewModel.Id;

    [SerializeField] private GameObject _selectionBorder;
    private BuildingViewModel _viewModel;

    public void Setup(BuildingViewModel viewModel)
    {
      _viewModel = viewModel;

      viewModel.Selected
        .Subscribe(_ => ShowSelectionBorder())
        .AddTo(this);

      viewModel.Unselected
        .Subscribe(_ => HideSelectionBorder())
        .AddTo(this);

      viewModel.Destroyed
        .Subscribe(_ => Destroy(gameObject))
        .AddTo(this);
    }

    private void ShowSelectionBorder() =>
      _selectionBorder.SetActive(true);

    private void HideSelectionBorder() =>
      _selectionBorder.SetActive(false);


  }
}