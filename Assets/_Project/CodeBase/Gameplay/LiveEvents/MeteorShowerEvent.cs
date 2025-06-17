using System;
using _Project.CodeBase.Data.Remote;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Services.DateTimeService;
using _Project.CodeBase.Services.RemoteConfigsService;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.GameEvents
{
  public class MeteorShowerEvent : IDisposable
  {
    private const string RemoteConfigKey = "key";

    private readonly IRemoteConfigService _remoteConfigService;
    private readonly MeteoriteSpawner _meteoriteSpawner;
    private readonly IDateTimeService _dateTimeService;
    private readonly CompositeDisposable _disposable = new();
    private readonly ReactiveProperty<bool> _isActive = new(false);

    private MeteorShowerEventData _eventData;
    private DateTime _eventStartUtc;
    private DateTime _eventEndUtc;

    public MeteorShowerEvent(IRemoteConfigService remoteConfigService, MeteoriteSpawner meteoriteSpawner,
      IDateTimeService dateTimeService)
    {
      _remoteConfigService = remoteConfigService;
      _meteoriteSpawner = meteoriteSpawner;
      _dateTimeService = dateTimeService;
    }

    public async UniTask InitializeAsync()
    {
      await UniTask.WhenAll(_remoteConfigService.WhenReady, _dateTimeService.WhenReady);

      if (!_dateTimeService.IsServerTimeAvailable)
        return;

      string json = _remoteConfigService.GetValue<string>(RemoteConfigKey);

      if (string.IsNullOrEmpty(json))
        return;

      _eventData = JsonConvert.DeserializeObject<MeteorShowerEventData>(json);

      if (_eventData == null || !_eventData.Enabled)
        return;

      _eventStartUtc = DateTime.Parse(_eventData.StartUtc);
      _eventEndUtc = DateTime.Parse(_eventData.EndUtc);

      _dateTimeService.ServerTime
        .Subscribe(EvaluateActivation)
        .AddTo(_disposable);

      _isActive.Subscribe(value =>
        {
          if (value)
            _meteoriteSpawner.Start();
          else
            _meteoriteSpawner.Stop();
        })
        .AddTo(_disposable);
    }

    public void Dispose() => 
      _disposable?.Dispose();

    private void EvaluateActivation(DateTime currentTime) =>
      _isActive.Value = currentTime >= _eventStartUtc && currentTime < _eventEndUtc;
  }
}