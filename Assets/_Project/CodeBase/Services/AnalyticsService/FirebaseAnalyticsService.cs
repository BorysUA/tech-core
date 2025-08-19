using System;
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

    public void LogEvent(string name, params EventParameter[] parameters)
    {
      if (parameters.Length == 0)
      {
        FirebaseAnalytics.LogEvent(name);
        return;
      }

      Parameter[] builder = new Parameter[parameters.Length];

      for (int i = 0; i < parameters.Length; i++)
      {
        builder[i] = parameters[i].ParameterType switch
        {
          EventParameterType.Long => new Parameter(parameters[i].Key, parameters[i].Long),
          EventParameterType.Double => new Parameter(parameters[i].Key, parameters[i].Double),
          EventParameterType.String => new Parameter(parameters[i].Key, parameters[i].String),
          _ => throw new ArgumentOutOfRangeException()
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