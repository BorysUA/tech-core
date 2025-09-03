using System;
using _Project.CodeBase.Services.LogService;
using UnityEngine;
using static UnityEngine.Screen;

namespace _Project.CodeBase.Gameplay.Services.CameraSystem
{
  public class CoordinateMapper
  {
    private readonly ILogService _logService;
    private readonly ICameraProvider _cameraRigAgent;

    private readonly Vector2 _centerScreen = new(width / 2f, height / 2f);
    private Plane _groundPlane = new(Vector3.up, Vector3.zero);

    public CoordinateMapper(ILogService logService, ICameraProvider cameraRigAgent)
    {
      _logService = logService;
      _cameraRigAgent = cameraRigAgent;
    }

    public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
    {
      Ray screenPointToRay = _cameraRigAgent.Camera.ScreenPointToRay(screenPoint);
      return CastRayFromCameraPoint(screenPointToRay);
    }

    public Vector3 CenterScreenToWorldPoint()
    {
      Ray screenPointToRay = _cameraRigAgent.Camera.ScreenPointToRay(_centerScreen);
      return CastRayFromCameraPoint(screenPointToRay);
    }

    public Vector3 WorldToScreenPoint(Vector3 worldPosition)
    {
      return _cameraRigAgent.Camera.WorldToScreenPoint(worldPosition);
    }

    private Vector3 CastRayFromCameraPoint(Ray ray)
    {
      if (_groundPlane.Raycast(ray, out float enter))
      {
#if UNITY_EDITOR
        DebugRay(ray, enter);
#endif
        return ray.GetPoint(enter);
      }

      _logService.LogError(GetType(),
        "Raycast did not cross the ground plane. Check the camera parameters or the position of the ground plane.",
        new InvalidOperationException());

      return default;
    }

    private void DebugRay(Ray ray, float distance)
    {
      Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 0.01f);
    }
  }
}