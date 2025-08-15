using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Trade
{
  public readonly struct FulfillTradeOfferCommand : ICommand<bool>
  {
    public int BuildingId { get; }

    public FulfillTradeOfferCommand(int buildingId)
    {
      BuildingId = buildingId;
    }
  }
}