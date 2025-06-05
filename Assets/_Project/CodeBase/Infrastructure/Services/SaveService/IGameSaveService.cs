using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services
{
  public interface IGameSaveService
  {
    public UniTask SaveManualAsync(CancellationToken token = default);
  }
}