using System;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public interface IRemoteConfigService
  {
    public T GetValue<T>(string key);
    public object GetValue(string key, Type targetType);
  }
}