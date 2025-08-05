using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
{
  public readonly struct OpenTradeOfferCommand : ICommand<Unit>
  {
    public int BuildingId { get; }

    public OpenTradeOfferCommand(int buildingId)
    {
      BuildingId = buildingId;
    }
  }
}