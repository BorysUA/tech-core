using System;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public interface IAnalyticsService : IDisposable
  {
    public void SetUserProperty(string name, string value);
    public void SetEnabled(bool enabled);
    void LogEvent(string name, params EventParameter[] parameters);
  }
}