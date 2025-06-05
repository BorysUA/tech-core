using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace _Project.CodeBase.Services.LogService
{
  public class LogService : ILogService
  {
    public void LogInfo(Type sourceType, string message, [CallerMemberName] string callerName = "")
    {
      Debug.Log($"[{sourceType.Name}] [{callerName}] [INFO] {message}");
    }

    public void LogWarning(Type sourceType, string message, [CallerMemberName] string callerName = "")
    {
      Debug.LogWarning($"[{sourceType.Name}] [{callerName}]  [WARNING] {message}");
    }

    public void LogError(Type sourceType, string message, Exception exception = null,
      [CallerMemberName] string callerName = "")
    {
      string fullMessage = $"[{sourceType.Name}] [{callerName}]  [ERROR] {message}";

      if (exception != null)
        fullMessage += $"\nException: {exception.Message}\n{exception.StackTrace}";

      Debug.LogError(fullMessage);
    }
  }
}