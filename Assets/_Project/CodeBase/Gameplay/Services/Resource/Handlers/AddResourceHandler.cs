using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Gameplay.Services.Resource.Results;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class AddResourceHandler : ICommandHandler<AddResourceCommand, AddResourceResult>
  {
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;

    public AddResourceHandler(ILogService logService, IProgressService progressService)
    {
      _logService = logService;
      _progressService = progressService;
    }

    public AddResourceResult Execute(AddResourceCommand command)
    {
      if (_progressService.GameStateProxy.Resources.TryGetValue(command.Kind, out ResourceProxy resource))
      {
        resource.Amount.OnNext(resource.Amount.CurrentValue + command.Amount);
        return new AddResourceResult(true, command.Amount);
      }

      _logService.LogError(GetType(), $"Resource of type {command.Kind} not found.");
      return new AddResourceResult(false);
    }
  }
}