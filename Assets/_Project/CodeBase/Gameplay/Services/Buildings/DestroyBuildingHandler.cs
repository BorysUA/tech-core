using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Gameplay.Signals.Domain;
using Zenject;
using ObservableCollections;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class DestroyBuildingHandler : ICommandHandler<DestroyBuildingCommand>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;

    public DestroyBuildingHandler(IProgressService progressService, ILogService logService, SignalBus signalBus)
    {
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
    }

    public void Execute(DestroyBuildingCommand command)
    {
      ObservableDictionary<string, BuildingDataProxy> buildings = _progressService.GameStateProxy.BuildingsCollection;

      if (!buildings.TryGetValue(command.BuildingId, out BuildingDataProxy proxy))
      {
        _logService.LogWarning(GetType(),
          $"Attempt to destroy a non-existent building with the following id: {command.BuildingId}");
        return;
      }

      _signalBus.Fire(new BuildingDestroyed(proxy.Type, proxy.Level.CurrentValue));
      buildings.Remove(command.BuildingId);
    }
  }
}