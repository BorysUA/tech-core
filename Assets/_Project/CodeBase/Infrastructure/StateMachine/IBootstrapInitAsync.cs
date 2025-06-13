using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public interface IBootstrapInitAsync
  {
    public UniTask InitializeAsync();
  }
}