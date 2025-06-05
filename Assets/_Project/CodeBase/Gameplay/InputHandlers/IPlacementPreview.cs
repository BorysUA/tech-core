using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public interface IPlacementPreview
  {
    void Activate();
    void Deactivate();
    void SetPosition(Vector3 position);
  }
}