using System;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class NoneRemoteConfigService : IRemoteConfigServiceInternal
  {
    public DateTime LastFetchTime => default;

    public T GetValue<T>(string key) => 
      default;

    public object GetValue(string key, Type targetType) => 
      null;
  }
}