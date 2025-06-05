using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public readonly struct DestroyBuildingCommand : ICommand
  {
    public string BuildingId { get; }

    public DestroyBuildingCommand(string buildingId)
    {
      BuildingId = buildingId;
    }
  }
}