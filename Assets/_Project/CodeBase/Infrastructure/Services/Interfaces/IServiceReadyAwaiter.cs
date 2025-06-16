using _Project.CodeBase.Services;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IServiceReadyAwaiter
  {
    public UniTask WhenReady { get; }
  }
}