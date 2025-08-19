namespace _Project.CodeBase.Services.AnalyticsService
{
  public class NoneAnalyticsService : IAnalyticsService
  {
    public void SetUserProperty(string name, string value)
    {
    }

    public void SetEnabled(bool enabled)
    {
    }

    public void LogEvent(string name, params EventParameter[] parameters)
    {
    }

    public void Dispose()
    {
    }
  }
}