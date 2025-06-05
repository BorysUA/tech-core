using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Root
{
  public class PopUpsCanvas : MonoBehaviour
  {
    private Canvas _value;
    public RectTransform RectTransform { get; private set; }
    public Transform Root => transform;

    public void Awake()
    {
      _value = GetComponent<Canvas>();
      RectTransform = (RectTransform)_value.transform;
    }
  }
}