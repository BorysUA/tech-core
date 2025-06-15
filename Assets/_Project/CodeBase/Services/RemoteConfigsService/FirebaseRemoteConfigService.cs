using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.CodeBase.Services.RemoteConfigsService
{
  public class FirebaseRemoteConfigService : IRemoteConfigService
  {
    private readonly IFirebaseBootstrap _firebaseBootstrap;
    private readonly IAssetProvider _assetProvider;
    private readonly FirebaseRemoteConfig _remoteConfig = FirebaseRemoteConfig.DefaultInstance;

    private readonly Dictionary<Type, Func<ConfigValue, object>> _supportedTypes = new()
    {
      { typeof(string), configValue => configValue.StringValue },
      { typeof(int), configValue => configValue.LongValue },
      { typeof(long), configValue => configValue.LongValue },
      { typeof(bool), configValue => configValue.BooleanValue },
      { typeof(double), configValue => configValue.DoubleValue },
      { typeof(float), configValue => configValue.DoubleValue },
    };

    public FirebaseRemoteConfigService(IFirebaseBootstrap firebaseBootstrap, IAssetProvider assetProvider)
    {
      _firebaseBootstrap = firebaseBootstrap;
      _assetProvider = assetProvider;
    }

    public async UniTask<ServiceInitializationStatus> InitializeAsync()
    {
      DependencyStatus status = await _firebaseBootstrap.IsReady;

      if (status == DependencyStatus.Available)
      {
        TextAsset asset = await _assetProvider.LoadAssetAsyncFromResources<TextAsset>(AssetPath.RemoteConfigDefaults);
        Dictionary<string, object> defaults = JsonConvert.DeserializeObject<Dictionary<string, object>>(asset.text);

        await _remoteConfig.SetDefaultsAsync(defaults);
        await _remoteConfig.FetchAsync(TimeSpan.Zero);

        bool isActive = await _remoteConfig.ActivateAsync();

        if (isActive)
          return ServiceInitializationStatus.Succeeded;
      }

      return ServiceInitializationStatus.Failed;
    }

    public T GetValue<T>(string key) =>
      (T)GetValue(key, typeof(T));

    public object GetValue(string key, Type targetType)
    {
      ConfigValue configValue = _remoteConfig.GetValue(key);

      if (TryParse(configValue, targetType, out object result))
        return result;

      throw new NotSupportedException($"Remote config type '{targetType}' is not supported.");
    }

    private bool TryParse(ConfigValue configValue, Type targetType, out object result)
    {
      if (targetType.IsEnum)
      {
        result = Enum.Parse(targetType, configValue.StringValue ?? string.Empty, true);
        return true;
      }

      if (_supportedTypes.TryGetValue(targetType, out Func<ConfigValue, object> parser))
      {
        result = parser(configValue);
        return true;
      }

      result = null;
      return false;
    }
  }
}