using System;
using System.Collections;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.UI;
using _Project.CodeBase.Services.LogService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class SceneLoader : ISceneLoader
  {
    private readonly LoadScreen _loadScreen;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly ILogService _logService;

    public SceneLoader(LoadScreen loadScreen, ICoroutineRunner coroutineRunner, ILogService logService)
    {
      _loadScreen = loadScreen;
      _coroutineRunner = coroutineRunner;
      _logService = logService;
    }

    public void LoadScene(string sceneName, Action completed = null)
    {
      _logService.LogInfo(GetType(), $"Start loading {sceneName}");

      AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);

      if (loadSceneOperation == null)
      {
        _logService.LogError(GetType(),
          $"Scene {sceneName} not found in Build Settings – abort.");
        return;
      }

      loadSceneOperation.allowSceneActivation = true;

      loadSceneOperation.completed += _ =>
      {
        _logService.LogInfo(GetType(), $"{sceneName} loaded");
        completed?.Invoke();
      };

      _coroutineRunner.ExecuteCoroutine(TrackProgress(loadSceneOperation, sceneName));
    }

    private IEnumerator TrackProgress(AsyncOperation asyncOperation, string sceneName)
    {
      _loadScreen.Open();
      int lastStep = -1;

      while (!asyncOperation.isDone)
      {
        float normalized = Mathf.Clamp01(asyncOperation.progress / 0.9f);
        int step = Mathf.FloorToInt(normalized * 10f);

        if (step != lastStep)
        {
          lastStep = step;
          _logService.LogInfo(GetType(),
            $"Loading {sceneName}: {step * 10}%");
        }

        _loadScreen.UpdateProgressBar(normalized);
        
        Debug.Log($"[Track] p={asyncOperation.progress:F2} done={asyncOperation.isDone}");
        yield return null;
      }

      Debug.Log($"{GetType().Name} : Call load screen close");
      _loadScreen.Close();
    }
  }
}