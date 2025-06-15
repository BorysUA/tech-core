using System;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public class RemoteKeyAttribute : Attribute
  {
    public readonly string Key;

    public RemoteKeyAttribute(string key)
    {
      Key = key;
    }
  }
}