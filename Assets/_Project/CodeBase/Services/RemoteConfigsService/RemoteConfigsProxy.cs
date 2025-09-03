using System;
using _Project.CodeBase.Infrastructure.Guards;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class RemoteConfigsProxy : IBootstrapInitAsync, IRemoteConfigService
  {
    private readonly ILogService _logService;
    private readonly FirebaseRemoteConfigService _firebase;
    private readonly NoneRemoteConfigService _none;
    private readonly UniTaskCompletionSource _whenReadyTcs = new();

    private IRemoteConfigServiceInternal _current;
    public UniTask WhenReady => _whenReadyTcs.Task.WithCycleGuard(this);
    public DateTime LastFetchTime => _current.LastFetchTime;

    public RemoteConfigsProxy(FirebaseRemoteConfigService firebase, NoneRemoteConfigService noneRemoteConfigService,
      ILogService logService)
    {
      _firebase = firebase;
      _none = noneRemoteConfigService;
      _logService = logService;

      _current = noneRemoteConfigService;
    }

    public async UniTask InitializeAsync()
    {
      ServiceInitializationStatus status = await _firebase.InitializeAsync();

      if (status == ServiceInitializationStatus.Succeeded)
        _current = _firebase;

      _whenReadyTcs.TrySetResult();
    }

    public object GetValue(string key, Type targetType) =>
      _current.GetValue(key, targetType);

    public T GetValue<T>(string key) =>
      _current.GetValue<T>(key);
  }
}