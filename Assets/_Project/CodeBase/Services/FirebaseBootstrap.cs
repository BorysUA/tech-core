using System.Threading.Tasks;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Firebase;

namespace _Project.CodeBase.Services
{
  public class FirebaseBootstrap : IFirebaseBootstrap, IBootstrapInitAsync
  {
    private readonly ILogService _logService;
    private readonly UniTaskCompletionSource<DependencyStatus> _whenReadyTcs = new();
    public UniTask<DependencyStatus> WhenReady => _whenReadyTcs.Task;

    public FirebaseBootstrap(ILogService logService)
    {
      _logService = logService;
    }

    public async UniTask InitializeAsync()
    {
      DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

      if (status != DependencyStatus.Available)
        _logService.LogError(GetType(), $"Could not resolve all FirebaseApp dependencies: {status}");

      _whenReadyTcs.TrySetResult(status);
    }
  }
}