using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.CameraSystem
{
  public class CameraRigAgent : MonoBehaviour
  {
    [SerializeField] private Transform _shakeContainer;
    [SerializeField] private Camera _mainCamera;

    public Camera Camera => _mainCamera;
    public Transform ShakeContainer => _shakeContainer;

    public bool IsVisible(Bounds bounds)
    {
      Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);
      return GeometryUtility.TestPlanesAABB(cameraPlanes, bounds);
    }

    public bool IsVisible(Vector3 position)
    {
      Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(position);
      return viewportPoint is { z: > 0, x: > 0 and < 1, y: > 0 and < 1 };
    }
  }
}