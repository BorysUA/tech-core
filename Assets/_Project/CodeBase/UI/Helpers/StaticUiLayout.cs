using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.UI.Helpers
{
  public class StaticUiLayout : MonoBehaviour
  {
    private void Awake()
    {
      StartCoroutine(DisableLayoutNextFrame());
    }

    private IEnumerator DisableLayoutNextFrame()
    {
      yield return new WaitForEndOfFrame();
      Disable<LayoutGroup>();
      Disable<ContentSizeFitter>();
      Disable<AspectRatioFitter>();
    }

    private void Disable<T>() where T : Behaviour
    {
      foreach (T component in GetComponentsInChildren<T>(true))
        component.enabled = false;
    }
  }
}