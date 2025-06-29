using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class SpendResourcesHandler : ICommandHandler<SpendResourcesCommand, bool>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;

    public SpendResourcesHandler(IProgressService progressService, ILogService logService, SignalBus signalBus)
    {
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
    }

    public bool Execute(SpendResourcesCommand command)
    {
      foreach (ResourceAmountData resourceToSpend in command.Resources)
      {
        if (!_progressService.GameStateProxy.Resources.TryGetValue(resourceToSpend.Kind,
              out ResourceProxy resource))
        {
          _logService.LogError(GetType(), $"Resource of type {resourceToSpend.Kind} not found.");
          return false;
        }

        if (resource.Amount.CurrentValue < resourceToSpend.Amount)
        {
          _logService.LogWarning(GetType(),
            $"Insufficient resource: {resourceToSpend.Kind}, requested {resourceToSpend.Amount} ");
          return false;
        }
      }

      foreach (ResourceAmountData resourceToSpend in command.Resources)
      {
        ResourceProxy resource = _progressService.GameStateProxy.Resources[resourceToSpend.Kind];
        resource.Amount.OnNext(resource.Amount.CurrentValue - resourceToSpend.Amount);

        _signalBus.Fire(
          new ResourceAmountChanged(resourceToSpend.Kind, -resourceToSpend.Amount, resource.Amount.CurrentValue));
      }

      return true;
    }
  }
}