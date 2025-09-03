using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.CameraSystem
{
  public class CameraProvider : ICameraProvider
  {
    private readonly CameraRigAgent _cameraRigAgent;

    public Camera Camera => _cameraRigAgent != null ? _cameraRigAgent.Camera : null;
    public Transform ShakeContainer => _cameraRigAgent != null ? _cameraRigAgent.ShakeContainer : null;

    public CameraProvider(CameraRigAgent cameraRigAgent) =>
      _cameraRigAgent = cameraRigAgent;

    public bool IsVisible(Bounds bounds) =>
      _cameraRigAgent != null && _cameraRigAgent.IsVisible(bounds);

    public bool IsVisible(Vector3 position) =>
      _cameraRigAgent != null && _cameraRigAgent.IsVisible(position);
  }
}