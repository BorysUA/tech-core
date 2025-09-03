using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Services.InputService;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class CameraMovement : PlayerInputHandler, ICameraMovement
  {
    private readonly CoordinateMapper _coordinateMapper;
    private readonly ICameraProvider _cameraRigAgent;

    private readonly ReactiveProperty<Vector3> _movementDelta = new();

    private Vector2 _startInputPoint;
    private Vector3 _cameraStartPosition;
    private bool _hasTouchStarted;

    public ReadOnlyReactiveProperty<Vector3> MovementDelta => _movementDelta;

    public CameraMovement(CoordinateMapper coordinateMapper, ICameraProvider cameraRigAgent)
    {
      _coordinateMapper = coordinateMapper;
      _cameraRigAgent = cameraRigAgent;
    }

    public override void OnTouchStarted(Vector2 inputPoint)
    {
      _startInputPoint = inputPoint;
      _cameraStartPosition = _cameraRigAgent.Camera.transform.position;
      _hasTouchStarted = true;
    }

    public override void OnTouchMoved(Vector2 inputPoint)
    {
      if (!_hasTouchStarted)
        return;

      Vector3 currentTouchWorldPosition = _coordinateMapper.ScreenToWorldPoint(inputPoint);
      Vector3 startTouchWorldPosition = _coordinateMapper.ScreenToWorldPoint(_startInputPoint);

      Vector3 movementDelta = currentTouchWorldPosition - startTouchWorldPosition;
      _cameraRigAgent.Camera.transform.position = _cameraStartPosition - movementDelta;

      _movementDelta.OnNext(movementDelta);
    }

    public override void OnTouchEnded() =>
      _hasTouchStarted = false;
  }
}