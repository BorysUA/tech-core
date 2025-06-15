using System;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class NoneRemoteConfigService : IRemoteConfigService
  {
    public T GetValue<T>(string key) => 
      default;

    public object GetValue(string key, Type targetType) => 
      null;
  }
}