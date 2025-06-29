using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Functions;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Services.DateTimeService
{
  public class DateTimeService : IDateTimeService, IBootstrapInitAsync, IDisposable
  {
    private readonly IFirebaseBootstrap _firebaseBootstrap;
    private readonly ILogService _logService;
    private readonly UniTaskCompletionSource _whenReadyTcs = new();
    private readonly ReactiveProperty<DateTime> _serverTime = new();
    private readonly ReactiveProperty<DateTime> _localTime = new();

    private FirebaseFunctions _firebaseFunctions;

    private double _secondsUntilSync;
    private DateTime _serverTimeWhenSync;

    public UniTask WhenReady => _whenReadyTcs.Task;
    
    public ReadOnlyReactiveProperty<DateTime> CurrentTime => Observable
      .Interval(TimeSpan.FromSeconds(1))
      .Select(_ =>
        IsServerTimeAvailable
          ? _serverTimeWhenSync + TimeSpan.FromSeconds(Time.realtimeSinceStartup - _secondsUntilSync)
          : DateTime.Now)
      .Publish()
      .RefCount()
      .ToReadOnlyReactiveProperty();
    
    public bool IsServerTimeAvailable => !_serverTimeWhenSync.Equals(default);

    public DateTimeService(IFirebaseBootstrap firebaseBootstrap, ILogService logService)
    {
      _firebaseBootstrap = firebaseBootstrap;
      _logService = logService;
    }

    public async UniTask InitializeAsync()
    {
      await SetupServerTime();
      _whenReadyTcs.TrySetResult();
    }

    public void Dispose()
    {
      _serverTime?.Dispose();
      _localTime?.Dispose();
    }

    private async UniTask SetupServerTime()
    {
      DependencyStatus status = await _firebaseBootstrap.WhenReady;

      if (status == DependencyStatus.Available)
      {
        _firebaseFunctions = FirebaseFunctions.DefaultInstance;
        try
        {
          HttpsCallableResult result = await _firebaseFunctions.GetHttpsCallable("getServerTime").CallAsync();

          if (result.Data is Dictionary<object, object> data &&
              data.TryGetValue("serverUtc", out var serverUtcRaw) &&
              serverUtcRaw is string isoTime)
          {
            _serverTimeWhenSync = DateTimeOffset.Parse(isoTime).UtcDateTime;
            _secondsUntilSync = Time.realtimeSinceStartup;
            
            _logService.LogInfo(GetType(), $" Server UTC time: {_serverTimeWhenSync}");
          }
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), "Failed to get server time", exception);
        }
      }
    }
  }
}