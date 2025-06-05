using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public interface IOnLoadInitializableAsync
  {
    public UniTask InitializeAsync();
  }
}