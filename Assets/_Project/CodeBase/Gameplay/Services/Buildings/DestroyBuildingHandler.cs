using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using ObservableCollections;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class DestroyBuildingHandler : ICommandHandler<DestroyBuildingCommand>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;

    public DestroyBuildingHandler(IProgressService progressService)
    {
      _progressService = progressService;
    }

    public void Execute(DestroyBuildingCommand command)
    {
      ObservableDictionary<string, BuildingDataProxy> buildings = _progressService.GameStateProxy.BuildingsCollection;

      if (!buildings.ContainsKey(command.BuildingId))
        _logService.LogWarning(GetType(),
          $"Attempt to destroy a non-existent building with the following id: {command.BuildingId}");

      buildings.Remove(command.BuildingId);
    }
  }
}