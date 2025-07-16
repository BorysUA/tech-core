using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Services.InputService;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class GridPlacement : PlayerInputHandler
  {
    private readonly CoordinateMapper _coordinateMapper;

    private readonly ReactiveProperty<Vector3> _position = new();
    private readonly ReactiveProperty<PlacementState> _state = new();

    private PlacementPreview _preview;
    private Vector2Int _cellSize;
    private Func<IEnumerable<Vector2Int>, bool> _isPlacementValid;
    private List<Vector2Int> _currentPlace;

    public ReadOnlyReactiveProperty<PlacementState> State => _state;
    public ReadOnlyReactiveProperty<Vector3> Position => _position;

    public GridPlacement(CoordinateMapper coordinateMapper)
    {
      _coordinateMapper = coordinateMapper;
    }

    public void Setup(PlacementPreview preview, Vector3 spawnPoint, Vector2Int cellsSize,
      Func<IEnumerable<Vector2Int>, bool> isPlacementValid)
    {
      _preview = preview;
      _cellSize = cellsSize;
      _isPlacementValid = isPlacementValid;

      UpdateBuildingState(spawnPoint);
    }

    public override void OnTouchMoved(Vector2 inputPoint)
    {
      Vector3 groundPoint = _coordinateMapper.ScreenToWorldPoint(inputPoint);
      UpdateBuildingState(groundPoint);
    }

    public async UniTask<PlacementResult> ExecutePlacementAsync()
    {
      PlacementResult placementResult = await AwaitPlacementResult();

      Reset();

      return placementResult;
    }

    public void ConfirmPlace() =>
      _state.OnNext(PlacementState.Confirmed);

    public void StopPlacing() =>
      _state.OnNext(PlacementState.Cancelled);

    private void Reset()
    {
      _preview.Deactivate();
      _preview = null;
      _currentPlace = null;
      _isPlacementValid = null;
      _cellSize = default;
    }

    private async UniTask<PlacementResult> AwaitPlacementResult()
    {
      UniTask confirmTask = UniTask.WaitUntil(() => _state.CurrentValue == PlacementState.Confirmed);
      UniTask cancelTask = UniTask.WaitUntil(() => _state.CurrentValue == PlacementState.Cancelled);

      var completedTask = await UniTask.WhenAny(confirmTask, cancelTask);

      PlacementResult placementResult = new PlacementResult(_currentPlace, completedTask == 0);
      return placementResult;
    }

    private void UpdateBuildingState(Vector3 toPosition)
    {
      Vector3 snappedPos = GridUtils.GetSnappedPosition(toPosition, _cellSize);
      _currentPlace = GridUtils.GetCells(snappedPos, _cellSize);

      _preview.SetPosition(snappedPos);
      _position.OnNext(snappedPos);

      bool canBePlaced = _isPlacementValid(_currentPlace);

      _state.OnNext(canBePlaced
        ? PlacementState.PlacingValid
        : PlacementState.PlacingInvalid);
    }
  }
}