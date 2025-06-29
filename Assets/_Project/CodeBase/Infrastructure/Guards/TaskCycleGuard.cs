using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Guards
{
  public static class TaskCycleGuard
  {
    private static readonly AsyncLocal<Stack<object>> _asyncLocal = new();

    public static async UniTask WithCycleGuard(this UniTask task, object owner)
    {
      Stack<object> stack = _asyncLocal.Value ??= new Stack<object>();

      if (stack.Contains(owner))
      {
        string chain = string.Join("->", stack.Reverse().Append(owner));
        throw new Exception($"[WhenReady] Circular dependency: {chain}");
      }

      stack.Push(owner);

      try
      {
        await task;
      }
      finally
      {
        stack.Pop();
      }
    }
  }
}