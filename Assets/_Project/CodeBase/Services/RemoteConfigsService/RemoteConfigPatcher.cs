using System;
using System.Reflection;
using _Project.CodeBase.Infrastructure;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class RemoteConfigPatcher
  {
    private readonly IRemoteConfigService _remoteConfigService;
    private readonly ILogService _logService;
    private readonly ObjectFactory _objectFactory;

    public RemoteConfigPatcher(ILogService logService, IRemoteConfigService remoteConfigService,
      ObjectFactory objectFactory)
    {
      _remoteConfigService = remoteConfigService;
      _objectFactory = objectFactory;
      _logService = logService;
    }

    public T CreatePatchedProxy<T>(T config) where T : ScriptableObject
    {
      T configClone = _objectFactory.Instantiate(config);
      Type configType = typeof(T);

      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      foreach (MemberInfo memberInfo in configType.GetMembers(bindingFlags))
      {
        RemoteKeyAttribute attribute = memberInfo.GetCustomAttribute<RemoteKeyAttribute>();

        if (attribute == null)
          continue;

        Type targetType = memberInfo switch
        {
          FieldInfo fieldInfo => fieldInfo.FieldType,
          PropertyInfo propertyInfo => propertyInfo.PropertyType,
          _ => throw new NotSupportedException()
        };

        object configValue = _remoteConfigService.GetValue(attribute.Key, targetType);

        if (configValue == null)
        {
          _logService.LogWarning(GetType(),
            $"Config value for type {targetType} by key {attribute.Key} is null." +
            " Possibly none _remoteConfigService implementation.");
          continue;
        }

        if (!targetType.IsInstanceOfType(configValue))
        {
          _logService.LogWarning(GetType(),
            $"Type mismatch for field '{memberInfo.Name}': expected {targetType}, got {configValue.GetType()}");
          continue;
        }

        switch (memberInfo)
        {
          case PropertyInfo { CanWrite: true } property:
            property.SetValue(configClone, configValue);
            break;
          case FieldInfo field:
            field.SetValue(configClone, configValue);
            break;
        }
      }

      return configClone;
    }
  }
}