using System.Collections;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface ICoroutineRunner
  {
    Coroutine ExecuteCoroutine(IEnumerator enumerator);
    void TerminateCoroutine(IEnumerator enumerator);
    void TerminateCoroutine(Coroutine coroutine);
  }
}