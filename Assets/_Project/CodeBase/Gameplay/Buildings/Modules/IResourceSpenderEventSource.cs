using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Buildings.Modules
{
  public interface IResourceSpenderEventSource
  {
    Observable<AutoResetUniTaskCompletionSource<bool>> ResourceSpendRequested { get; }
  }
}