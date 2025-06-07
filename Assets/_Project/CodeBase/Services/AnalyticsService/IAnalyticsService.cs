using Firebase.Analytics;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public interface IAnalyticsService
  {
    public void LogEvent(string name, (string key, object value)[] parameters);
    public void SetUserProperty(string name, string value);
    public void SetEnabled(bool enabled);
  }
}