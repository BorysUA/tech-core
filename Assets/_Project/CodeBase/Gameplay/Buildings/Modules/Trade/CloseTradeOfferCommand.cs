using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Trade
{
  public readonly struct CloseTradeOfferCommand : ICommand<Unit>
  {
    public int BuildingId { get; }

    public CloseTradeOfferCommand(int buildingId)
    {
      BuildingId = buildingId;
    }
  }
}