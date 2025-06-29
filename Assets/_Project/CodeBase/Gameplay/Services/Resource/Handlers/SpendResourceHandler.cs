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
  public class SpendResourceHandler : ICommandHandler<SpendResourceCommand, bool>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly SignalBus _signalBus;

    public SpendResourceHandler(IProgressService progressService, ILogService logService, SignalBus signalBus)
    {
      _progressService = progressService;
      _logService = logService;
      _signalBus = signalBus;
    }

    public bool Execute(SpendResourceCommand command)
    {
      if (!_progressService.GameStateProxy.Resources.TryGetValue(command.Kind, out ResourceProxy resource))
      {
        _logService.LogError(GetType(), $"Resource of type {command.Kind} not found.");
        return false;
      }

      if (resource.Amount.CurrentValue < command.Amount)
      {
        _logService.LogWarning(GetType(), $"Insufficient resources: {command.Amount} requested, but not available.");
        return false;
      }

      _signalBus.Fire(new ResourceAmountChanged(command.Kind, -command.Amount, resource.Amount.CurrentValue));
      resource.Amount.OnNext(resource.Amount.CurrentValue - command.Amount);
      return true;
    }
  }
}