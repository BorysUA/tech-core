using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Zenject;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class AddResourceHandler : ICommandHandler<AddResourceCommand, AddResourceResult>
  {
    private readonly IProgressService _progressService;
    private readonly SignalBus _signalBus;
    private readonly ILogService _logService;

    public AddResourceHandler(ILogService logService, IProgressService progressService, SignalBus signalBus)
    {
      _logService = logService;
      _progressService = progressService;
      _signalBus = signalBus;
    }

    public AddResourceResult Execute(AddResourceCommand command)
    {
      if (_progressService.GameStateProxy.Resources.TryGetValue(command.Kind, out ResourceProxy resource))
      {
        resource.Amount.OnNext(resource.Amount.CurrentValue + command.Amount);

        _signalBus.Fire(new ResourceAmountChanged(command.Kind, command.Amount, resource.Amount.CurrentValue));

        return new AddResourceResult(true, command.Amount);
      }

      _logService.LogError(GetType(), $"Resource of type {command.Kind} not found.");
      return new AddResourceResult(false);
    }
  }
}