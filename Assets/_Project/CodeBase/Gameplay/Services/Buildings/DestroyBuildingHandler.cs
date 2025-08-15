using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class DestroyBuildingHandler : ICommandHandler<DestroyBuildingCommand, Unit>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;

    public DestroyBuildingHandler(IProgressWriter progressService, ILogService logService, SignalBus signalBus)
    {
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
    }

    public Unit Execute(in DestroyBuildingCommand command)
    {
      var buildings = _progressService.GameStateModel.WriteOnlyBuildings;

      if (!buildings.TryGetValue(command.BuildingId, out IBuildingDataWriter proxy))
      {
        _logService.LogWarning(GetType(),
          $"Attempt to destroy a non-existent building with the following id: {command.BuildingId}");
        return Unit.Default;
      }

      BuildingDestroyed buildingDestroyed = new BuildingDestroyed(proxy.Id, proxy.Type, proxy.Level.CurrentValue);
      buildings.Remove(command.BuildingId);

      _signalBus.Fire(buildingDestroyed);

      return Unit.Default;
    }
  }
}