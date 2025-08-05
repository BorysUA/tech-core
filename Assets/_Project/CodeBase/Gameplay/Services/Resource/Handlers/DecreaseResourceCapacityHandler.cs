using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class DecreaseResourceCapacityHandler : ICommandHandler<DecreaseResourceCapacityCommand, bool>
  {
    private readonly IProgressWriter _progressService;
    private readonly ILogService _logService;

    public DecreaseResourceCapacityHandler(
      IProgressWriter progressService,
      ILogService logService)
    {
      _progressService = progressService;
      _logService = logService;
    }

    public bool Execute(in DecreaseResourceCapacityCommand command)
    {
      if (command.CapacityDelta <= 0)
      {
        _logService.LogWarning(GetType(), $"Invalid decrease amount: {command.CapacityDelta}");
        return false;
      }

      if (!_progressService.GameStateModel.WriteOnlyResources.TryGetValue(command.ResourceKind,
            out IResourceWriter resourceWriter))
      {
        _logService.LogWarning(GetType(), $"Resource {command.ResourceKind} not found.");
        return false;
      }

      resourceWriter.Capacity.Value = Mathf.Max(resourceWriter.Capacity.Value - command.CapacityDelta, 0);
      return true;
    }
  }
}