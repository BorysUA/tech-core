using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public readonly struct DestroyBuildingCommand : ICommand<Unit>
  {
    public int BuildingId { get; }

    public DestroyBuildingCommand(int buildingId)
    {
      BuildingId = buildingId;
    }
  }
}