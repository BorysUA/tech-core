using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public class AnalyticsServiceProxy : IAnalyticsService, IBootstrapInitAsync
  {
    private const int BufferLimit = 20;

    private readonly FirebaseAnalyticsService _firebaseAnalytics;
    private readonly NoneAnalyticsService _noneAnalytics;
    private readonly ILogService _logService;

    private readonly Queue<Action<IAnalyticsService>> _buffer = new(BufferLimit);

    private IAnalyticsService _current;

    public AnalyticsServiceProxy(FirebaseAnalyticsService firebaseAnalytics, NoneAnalyticsService noneAnalytics,
      ILogService logService)
    {
      _firebaseAnalytics = firebaseAnalytics;
      _noneAnalytics = noneAnalytics;
      _logService = logService;

      _current = noneAnalytics;
    }

    public async UniTask InitializeAsync()
    {
      ServiceInitializationStatus status = await _firebaseAnalytics.InitializeAsync();
#if !UNITY_EDITOR
      if (status == ServiceInitializationStatus.Success)
      {
        _current = _firebaseAnalytics;
        FlushBuffer();
      }
#endif
    }

    public void Dispose() => 
      _current.Dispose();

    public void LogEvent(string name, params (string key, object value)[] parameters)
      => ForwardOrBuffer(service => service.LogEvent(name, parameters));

    public void SetUserProperty(string name, string value)
      => ForwardOrBuffer(service => service.SetUserProperty(name, value));

    public void SetEnabled(bool enabled)
      => _current.SetEnabled(enabled);

    private void ForwardOrBuffer(Action<IAnalyticsService> call)
    {
      if (_current is NoneAnalyticsService)
      {
        if (_buffer.Count >= BufferLimit)
        {
          _buffer.Dequeue();
          _logService.LogWarning(GetType(),
            $"Analytics buffer overflow – oldest event dropped (limit: {BufferLimit})");
        }

        _buffer.Enqueue(call);
      }
      else
      {
        call(_current);
      }
    }

    private void FlushBuffer()
    {
      while (_buffer.Count > 0)
        _buffer.Dequeue().Invoke(_current);
    }
  }
}