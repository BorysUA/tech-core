using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using ObservableCollections;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class DecreaseResourceCapacityHandler : ICommandHandler<DecreaseResourceCapacityCommand, bool>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;

    public DecreaseResourceCapacityHandler(
      IProgressService progressService,
      ILogService logService)
    {
      _progressService = progressService;
      _logService = logService;
    }

    public bool Execute(DecreaseResourceCapacityCommand command)
    {
      if (command.CapacityDelta <= 0)
      {
        _logService.LogWarning(GetType(), $"Invalid decrease amount: {command.CapacityDelta}");
        return false;
      }

      ObservableDictionary<ResourceKind, ResourceProxy> resources = _progressService.GameStateProxy.Resources;

      if (!resources.TryGetValue(command.ResourceKind, out ResourceProxy proxy))
      {
        _logService.LogWarning(GetType(), $"Resource {command.ResourceKind} not found.");
        return false;
      }

      proxy.Capacity.Value = Mathf.Max(proxy.Capacity.Value - command.CapacityDelta, 0);
      return true;
    }
  }
}