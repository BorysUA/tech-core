using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Firebase;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public class AnalyticsServiceProxy : IAnalyticsService, IOnLoadInitializableAsync
  {
    private readonly FirebaseAnalyticsService _firebaseAnalytics;
    private readonly NoneAnalyticsService _noneAnalytics;
    private readonly ILogService _logService;

    private IAnalyticsService _current;

    public AnalyticsServiceProxy(FirebaseAnalyticsService firebaseAnalytics, NoneAnalyticsService noneAnalytics,
      ILogService logService)
    {
      _firebaseAnalytics = firebaseAnalytics;
      _noneAnalytics = noneAnalytics;
      _logService = logService;
    }

    public async UniTask InitializeAsync()
    {
      DependencyStatus status = await _firebaseAnalytics.InitializeAsync();
      
      if (status == DependencyStatus.Available)
        _current = _firebaseAnalytics;
      else
      {
        _current = _noneAnalytics;
        _logService.LogError(GetType(), $"Could not resolve all FirebaseApp dependencies: {status}");
      }
    }

    public void LogEvent(string name, (string key, object value)[] parameters) =>
      _current.LogEvent(name, parameters);

    public void SetUserProperty(string name, string value) =>
      _current.SetUserProperty(name, value);

    public void SetEnabled(bool enabled) =>
      _current.SetEnabled(enabled);
  }
}