using _Project.CodeBase.Infrastructure.Services.Interfaces;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public interface IBootstrapInitAsync 
  {
    public UniTask InitializeAsync();
  }
}