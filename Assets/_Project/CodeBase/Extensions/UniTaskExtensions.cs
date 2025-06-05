using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Extensions
{
  public static class UniTaskExtensions
  {
    public static async UniTask<T> ContinueOnMainThread<T>(this UniTask<T> task)
    {
      T result = await task;
      await UniTask.SwitchToMainThread();
      return result;
    }

    public static async UniTask ContinueOnMainThread(this UniTask task)
    {
      await task;
      await UniTask.SwitchToMainThread();
    }
  }
}