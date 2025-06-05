using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Root
{
  public class WindowsCanvas : MonoBehaviour
  {
    public Canvas Value { get; private set; }
    public Transform Root => transform;

    public void Awake()
    {
      Value = GetComponent<Canvas>();
    }
  }
}