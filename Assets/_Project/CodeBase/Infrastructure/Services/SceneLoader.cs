using System;
using System.Collections;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class SceneLoader : ISceneLoader
  {
    private readonly LoadScreen _loadScreen;
    private readonly ICoroutineRunner _coroutineRunner;

    public SceneLoader(LoadScreen loadScreen, ICoroutineRunner coroutineRunner)
    {
      _loadScreen = loadScreen;
      _coroutineRunner = coroutineRunner;
    }

    public void LoadScene(string sceneName, Action completed = null)
    {
      AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
      asyncOperation.completed += operation => completed?.Invoke();

      _coroutineRunner.ExecuteCoroutine(ShowLoadProgress(asyncOperation));
    }

    private IEnumerator ShowLoadProgress(AsyncOperation asyncOperation)
    {
      _loadScreen.Open();

      while (!asyncOperation.isDone)
      {
        _loadScreen.UpdateProgressBar(Mathf.Clamp01(asyncOperation.progress / 0.9f));
        yield return null;
      }

      _loadScreen.Close();
    }
  }
}