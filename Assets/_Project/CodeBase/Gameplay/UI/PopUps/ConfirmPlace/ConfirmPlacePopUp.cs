using _Project.CodeBase.UI.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace
{
  public class ConfirmPlacePopUp : BasePopUp<ConfirmPlaceViewModel>
  {
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private RectTransform _rectTransform;

    public override void Setup(ConfirmPlaceViewModel viewModel)
    {
      base.Setup(viewModel);

      _confirmButton.OnClickAsObservable()
        .Subscribe(_ => viewModel.PlaceBuilding())
        .AddTo(this);
      
      _cancelButton.OnClickAsObservable()
        .Subscribe(_ => viewModel.StopPlacing())
        .AddTo(this);
      
      viewModel.Point
        .Subscribe(SetPosition)
        .AddTo(this);

      viewModel.IsPlacementValid
        .Subscribe(isValid =>
        {
          if (isValid)
            Unlock();
          else
            Block();
        })
        .AddTo(this);
    }

    private void Unlock() =>
      _confirmButton.interactable = true;

    private void Block() =>
      _confirmButton.interactable = false;

    private void SetPosition(Vector2 point) =>
      _rectTransform.anchoredPosition = point;
  }
}