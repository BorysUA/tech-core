using R3;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public class NoneAnalyticsService : IAnalyticsService
  {
    public void LogEvent(string name, (string key, object value)[] parameters)
    {
    }

    public void SetUserProperty(string name, string value)
    {
    }

    public void SetEnabled(bool enabled)
    {
    }
    
  }
}