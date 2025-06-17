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
  public class DateTimeService : IBootstrapInitAsync, IDateTimeService, IDisposable
  {
    private readonly FirebaseBootstrap _firebaseBootstrap;
    private readonly ILogService _logService;
    private readonly UniTaskCompletionSource _whenReadyTcs = new();
    private readonly ReactiveProperty<DateTime> _serverTime = new();
    private readonly ReactiveProperty<DateTime> _localTime = new();
    private readonly CompositeDisposable _disposable = new();

    private FirebaseFunctions _firebaseFunctions;

    private double _secondsUntilSync;
    private DateTime _serverTimeWhenSync;

    public UniTask WhenReady => _whenReadyTcs.Task;
    public ReadOnlyReactiveProperty<DateTime> ServerTime => _serverTime;
    public ReadOnlyReactiveProperty<DateTime> LocalTime => _localTime;
    public bool IsServerTimeAvailable => !_serverTimeWhenSync.Equals(default);

    public DateTimeService(FirebaseBootstrap firebaseBootstrap, ILogService logService)
    {
      _firebaseBootstrap = firebaseBootstrap;
      _logService = logService;
    }

    public async UniTask InitializeAsync()
    {
      SetupLocalTime();
      await SetupServerTime();
      _whenReadyTcs.TrySetResult();
    }

    public void Dispose()
    {
      _serverTime?.Dispose();
      _localTime?.Dispose();
      _disposable?.Dispose();
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

          if (result.Data is Dictionary<string, object> data &&
              data.TryGetValue("serverUtc", out object serverUtcRaw) &&
              serverUtcRaw is string isoTime)
          {
            _serverTimeWhenSync = DateTimeOffset.Parse(isoTime).UtcDateTime;
            _secondsUntilSync = Time.realtimeSinceStartup;

            Observable
              .EveryUpdate()
              .Subscribe(_ => UpdateServerTime())
              .AddTo(_disposable);

            _logService.LogInfo(GetType(), $" Server UTC time: {ServerTime}");
          }
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), "Failed to get server time", exception);
        }
      }
    }

    private void SetupLocalTime()
    {
      Observable
        .EveryUpdate()
        .Subscribe(_ => UpdateLocalTime())
        .AddTo(_disposable);
    }

    private void UpdateServerTime() => 
      _serverTime.Value = _serverTimeWhenSync + TimeSpan.FromSeconds(Time.realtimeSinceStartup - _secondsUntilSync);

    private void UpdateLocalTime() =>
      _localTime.Value = DateTime.Now;
  }
}