using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public class FirebaseAnalyticsService : IAnalyticsService
  {
    private readonly IFirebaseBootstrap _firebaseBootstrap;

    public FirebaseAnalyticsService(IFirebaseBootstrap firebaseBootstrap)
    {
      _firebaseBootstrap = firebaseBootstrap;
    }

    public async UniTask<ServiceInitializationStatus> InitializeAsync()
    {
      DependencyStatus dependencyStatus = await _firebaseBootstrap.WhenReady;

      if (dependencyStatus == DependencyStatus.Available)
        SetEnabled(true);

      return dependencyStatus == DependencyStatus.Available
        ? ServiceInitializationStatus.Succeeded
        : ServiceInitializationStatus.Failed;
    }

    public void LogEvent(string name, params (string key, object value)[] parameters)
    {
      if (parameters.Length == 0)
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