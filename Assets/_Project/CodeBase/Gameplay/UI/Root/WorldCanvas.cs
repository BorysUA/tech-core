using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Root
{
  public class WorldCanvas : MonoBehaviour
  {
    public Canvas Value { get; private set; }
    public Transform Root => transform;

    public void Awake()
    {
      Value = GetComponent<Canvas>();
    }
  }
}