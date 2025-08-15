using System;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using R3;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace
{
  public class ConfirmPlaceViewModel : BasePopUpViewModel, IDisposable
  {
    private readonly GridPlacement _gridPlacement;
    private readonly CompositeDisposable _compositeDisposable = new();

    public ReadOnlyReactiveProperty<bool> IsPlacementValid => _isPlacementValid;
    public ReadOnlyReactiveProperty<Vector2> Point => _point;

    private readonly ReactiveProperty<bool> _isPlacementValid = new();
    private readonly ReactiveProperty<Vector2> _point = new();

    public ConfirmPlaceViewModel(GridPlacement gridPlacement,
      IWindowsService windowsService, IPopUpService popUpService, CoordinateMapper coordinateMapper)
    {
      _gridPlacement = gridPlacement;

      _gridPlacement.State
        .Subscribe(state =>
        {
          if (state is PlacementState.Cancelled or PlacementState.Confirmed)
            Hide();
          else
            _isPlacementValid.OnNext(state is PlacementState.PlacingValid);
        })
        .AddTo(_compositeDisposable);

      windowsService.WindowOpened
        .Subscribe(_ =>
        {
          if (_gridPlacement.State.CurrentValue is PlacementState.PlacingInvalid or PlacementState.PlacingValid)
            StopPlacing();
        })
        .AddTo(_compositeDisposable);

      _gridPlacement.Position
        .Subscribe(worldPosition =>
        {
          Vector3 screenPoint = coordinateMapper.WorldToScreenPoint(worldPosition);
          if (popUpService.ScreenToCanvasPoint(screenPoint, out Vector2 localPoint))
            _point.OnNext(localPoint);
        })
        .AddTo(_compositeDisposable);
    }

    public void PlaceBuilding() =>
      _gridPlacement.ConfirmPlace();

    public void StopPlacing() => 
      _gridPlacement.StopPlacing();

    public void Dispose() =>
      _compositeDisposable.Dispose();
  }
}