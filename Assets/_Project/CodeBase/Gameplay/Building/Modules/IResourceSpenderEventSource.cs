using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public interface IResourceSpenderEventSource
  {
    Observable<AutoResetUniTaskCompletionSource<bool>> ResourceSpendRequested { get; }
  }
}