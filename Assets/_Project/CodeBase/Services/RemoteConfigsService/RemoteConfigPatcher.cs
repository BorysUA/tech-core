using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _Project.CodeBase.Infrastructure.Guards;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class RemoteConfigPatcher : IServiceReadyAwaiter
  {
    private readonly IRemoteConfigService _remoteConfigService;
    private readonly ILogService _logService;
    private readonly ObjectFactory _objectFactory;

    private readonly Dictionary<Type, MemberInfo[]> _cachedMembers = new();
    public UniTask WhenReady => _remoteConfigService.WhenReady.WithCycleGuard(this);

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

      foreach (MemberInfo memberInfo in GetPatchedConfigMembers(typeof(T)))
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

    private IEnumerable<MemberInfo> GetPatchedConfigMembers(Type configType)
    {
      if (_cachedMembers.TryGetValue(configType, out MemberInfo[] cachedMembers))
        return cachedMembers;

      BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      List<MemberInfo> members = new List<MemberInfo>();

      members.AddRange(configType.GetFields(bindingFlags)
        .Where(member => member.IsDefined(typeof(RemoteKeyAttribute))));

      members.AddRange(configType.GetProperties(bindingFlags)
        .Where(member => member.CanWrite && member.IsDefined(typeof(RemoteKeyAttribute))));

      _cachedMembers.Add(configType, members.ToArray());

      return members;
    }
  }
}