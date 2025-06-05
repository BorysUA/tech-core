using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class PlacementPreview : MonoBehaviour
  {
    public void SetPosition(Vector3 position) =>
      transform.position = position;

    public void Activate() =>
      gameObject.SetActive(true);

    public void Deactivate() =>
      gameObject.SetActive(false);
  }
}