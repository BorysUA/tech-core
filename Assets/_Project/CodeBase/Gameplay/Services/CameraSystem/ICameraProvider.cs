using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.CameraSystem
{
  public interface ICameraProvider
  {
    public Camera Camera { get; }
    public Transform ShakeContainer { get; }

    public bool IsVisible(Bounds bounds);
    public bool IsVisible(Vector3 position);
  }
}