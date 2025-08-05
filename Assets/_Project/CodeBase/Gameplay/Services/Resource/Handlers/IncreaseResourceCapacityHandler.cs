using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class IncreaseResourceCapacityHandler : ICommandHandler<IncreaseResourceCapacityCommand, bool>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;

    public IncreaseResourceCapacityHandler(
      IProgressWriter progressService,
      ILogService logService)
    {
      _progressService = progressService;
      _logService = logService;
    }

    public bool Execute(in IncreaseResourceCapacityCommand command)
    {
      if (command.CapacityDelta <= 0)
      {
        _logService.LogWarning(GetType(), $"Invalid increase amount: {command.CapacityDelta}");
        return false;
      }

      if (!_progressService.GameStateModel.WriteOnlyResources.TryGetValue(command.ResourceKind,
            out IResourceWriter proxy))
      {
        _logService.LogWarning(GetType(), $"Resource {command.ResourceKind} not found.");
        return false;
      }

      proxy.Capacity.Value += command.CapacityDelta;
      return true;
    }
  }
}