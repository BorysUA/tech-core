using _Project.CodeBase.Gameplay.GameEvents;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Services.RemoteConfigsService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Gameplay.Services
{
  public class LiveEventsService : ILiveEventsService, IGameplayInitAsync
  {
    private readonly IRemoteConfigService _remoteConfigService;
    private readonly MeteorShowerEvent _meteorShowerEvent;

    public LiveEventsService(IRemoteConfigService remoteConfigService, MeteorShowerEvent meteorShowerEvent)
    {
      _remoteConfigService = remoteConfigService;
      _meteorShowerEvent = meteorShowerEvent;
    }

    public async UniTask InitializeAsync()
    {
    }
  }
}