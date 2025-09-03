using System.Collections;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.Services.CoroutineProvider
{
  public class CoroutineProvider : ICoroutineProvider
  {
    private readonly CoroutineRunner _coroutineRunner;

    public CoroutineProvider(CoroutineRunner coroutineRunner)
    {
      _coroutineRunner = coroutineRunner;
    }

    public Coroutine ExecuteCoroutine(IEnumerator enumerator) =>
      _coroutineRunner.ExecuteCoroutine(enumerator);

    public void TerminateCoroutine(IEnumerator enumerator)
    {
      if (_coroutineRunner == null) 
        return;
      
      if (enumerator is null)
        return;

      _coroutineRunner.TerminateCoroutine(enumerator);
    }

    public void TerminateCoroutine(Coroutine coroutine)
    {
      if (_coroutineRunner == null) 
        return;
      
      if (coroutine is null)
        return;

      _coroutineRunner.TerminateCoroutine(coroutine);
    }
  }
}