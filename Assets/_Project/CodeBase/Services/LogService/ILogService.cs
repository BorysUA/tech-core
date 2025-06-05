using System;
using System.Runtime.CompilerServices;

namespace _Project.CodeBase.Services.LogService
{
  public interface ILogService
  {
    void LogInfo(Type sourceType, string message, [CallerMemberName] string callerName = "");
    void LogWarning(Type sourceType, string message, [CallerMemberName] string callerName = "");

    void LogError(Type sourceType, string message, Exception exception = null,
      [CallerMemberName] string callerName = "");
  }
}