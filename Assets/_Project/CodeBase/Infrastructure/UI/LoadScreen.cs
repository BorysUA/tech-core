using System.Collections;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Infrastructure.UI
{
  public class LoadScreen : MonoBehaviour
  {
    [SerializeField] private CanvasGroup _screen;
    [SerializeField] private Slider _slider;

    private ILogService _logService;

    [Inject]
    public void Construct(ILogService logService)
    {
      _logService = logService;
    }

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
      _logService.LogInfo(GetType(),"Starting screen fade in");
      while (_screen.alpha > 0f)
      {
        _screen.alpha -= Time.deltaTime;
        yield return null;
      }

      _logService.LogInfo(GetType(),"Loading screen was closed");
      gameObject.SetActive(false);
    }
  }
}