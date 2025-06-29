using System.Collections.Generic;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.UI.HUD.GameEvent
{
  public class GameEventsAddressMap
  {
    private readonly ILogService _logService;

    private readonly Dictionary<GameEventType, string> _map = new()
    {
      { GameEventType.MeteorShowerEvent, AssetAddress.MeteorShowerEventIndicator},
      { GameEventType.ProductionBoostEvent, AssetAddress.ProductionBoostEventIndicator},
    };

    public GameEventsAddressMap(ILogService logService) =>
      _logService = logService;

    public string GetAddress(GameEventType eventType)
    {
      if (_map.TryGetValue(eventType, out string address))
        return address;

      _logService.LogError(GetType(), $"No mapping for {eventType}");
      return null;
    }
  }
}