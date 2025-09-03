using System.Collections;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.Services.CoroutineProvider
{
  public class CoroutineRunner : MonoBehaviour
  {
    public Coroutine ExecuteCoroutine(IEnumerator enumerator) =>
      StartCoroutine(enumerator);

    public void TerminateCoroutine(IEnumerator enumerator) =>
      StopCoroutine(enumerator);

    public void TerminateCoroutine(Coroutine coroutine) =>
      StopCoroutine(coroutine);
  }
}