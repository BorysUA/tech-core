using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
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