using System.Collections;
using _Project.CodeBase.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Infrastructure.UI
{
  public class LoadScreen : MonoBehaviour
  {
    [SerializeField] private CanvasGroup _screen;
    [SerializeField] private Slider _slider;

    public void Open()
    {
      _slider.value = 0f;
      _screen.alpha = 1f;
      gameObject.SetActive(true);
    }

    public void UpdateProgressBar(float progress)
    {
      _slider.value = progress;
    }

    public void Close()
    {
      StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
      while (_screen.alpha > 0f)
      {
        _screen.alpha -= Time.deltaTime;
        yield return null;
      }

      gameObject.SetActive(false);
    }
  }
}