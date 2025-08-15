using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Health
{
  public readonly struct ApplyHealthDeltaCommand : ICommand<bool>
  {
    public int BuildingId { get; }
    public int Delta { get; }

    public ApplyHealthDeltaCommand(int buildingId, int delta)
    {
      BuildingId = buildingId;
      Delta = delta;
    }
  }
}