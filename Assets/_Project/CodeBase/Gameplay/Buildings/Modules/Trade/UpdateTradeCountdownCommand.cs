using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Trade
{
  public readonly struct UpdateTradeCountdownCommand : ICommand<Unit>
  {
    public int BuildingId { get; }
    public float OfferCloseCountdown { get; }
    public float NextOfferOpenCountdown { get; }

    public UpdateTradeCountdownCommand(int buildingId, float offerCloseCountdown,
      float nextOfferOpenCountdown)
    {
      BuildingId = buildingId;
      OfferCloseCountdown = offerCloseCountdown;
      NextOfferOpenCountdown = nextOfferOpenCountdown;
    }
  }
}