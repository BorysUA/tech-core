using System;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public class FirebaseAnalyticsService : IAnalyticsService, IDisposable
  {
    private readonly ILogService _logService;

    public FirebaseAnalyticsService(ILogService logService)
    {
      _logService = logService;
    }

    public async UniTask<DependencyStatus> InitializeAsync() =>
      await FirebaseApp.CheckAndFixDependenciesAsync();

    public void LogEvent(string name, (string key, object value)[] parameters)
    {
      if (parameters == null || parameters.Length == 0)
      {
        FirebaseAnalytics.LogEvent(name);
        return;
      }

      Parameter[] builder = new Parameter[parameters.Length];

      for (int i = 0; i < parameters.Length; i++)
      {
        (string key, object value) = parameters[i];

        builder[i] = value switch
        {
          int intValue => new Parameter(key, intValue),
          long longValue => new Parameter(key, longValue),
          float floatValue => new Parameter(key, floatValue),
          double doubleValue => new Parameter(key, doubleValue),
          bool boolValue => new Parameter(key, boolValue ? "true" : "false"),
          null => new Parameter(key, "null"),
          _ => new Parameter(key, value.ToString())
        };
      }

      FirebaseAnalytics.LogEvent(name, builder);
    }

    public void SetUserProperty(string name, string value) =>
      FirebaseAnalytics.SetUserProperty(name, value);

    public void SetEnabled(bool enabled) =>
      FirebaseAnalytics.SetAnalyticsCollectionEnabled(enabled);

    public void Dispose() =>
      FirebaseApp.DefaultInstance.Dispose();
  }
}