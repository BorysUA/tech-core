using Cysharp.Threading.Tasks;
using Firebase;

namespace _Project.CodeBase.Services
{
  public interface IFirebaseBootstrap
  {
    public UniTask<DependencyStatus> WhenReady { get; }
  }
}