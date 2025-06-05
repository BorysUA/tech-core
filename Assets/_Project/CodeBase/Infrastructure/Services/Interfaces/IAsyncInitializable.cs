using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IAsyncInitializable
  {
    public UniTask InitializeAsync();
  }
}