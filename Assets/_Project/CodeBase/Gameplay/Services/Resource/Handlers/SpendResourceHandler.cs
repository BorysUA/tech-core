using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class SpendResourceHandler : ICommandHandler<SpendResourceCommand, bool>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;

    public SpendResourceHandler(IProgressService progressService, ILogService logService)
    {
      _progressService = progressService;
      _logService = logService;
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

      resource.Amount.OnNext(resource.Amount.CurrentValue - command.Amount);
      return true;
    }
  }
}