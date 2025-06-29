using System;
using System.Collections;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
  {
    private ILogService _logService;

    [Inject]
    public void Construct(ILogService logService) =>
      _logService = logService;

    public Coroutine ExecuteCoroutine(IEnumerator enumerator) =>
      StartCoroutine(enumerator);

    public void TerminateCoroutine(IEnumerator enumerator)
    {
      if (enumerator is null)
        return;

      StopCoroutine(enumerator);
    }

    public void TerminateCoroutine(Coroutine coroutine)
    {
      if (coroutine is null)
        return;

      StopCoroutine(coroutine);
    }
    
  }
}