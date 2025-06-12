using Firebase.Analytics;
using R3;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public interface IAnalyticsService
  {
    public void LogEvent(string name, params (string key, object value)[] parameters);
    public void SetUserProperty(string name, string value);
    public void SetEnabled(bool enabled);
  }
}