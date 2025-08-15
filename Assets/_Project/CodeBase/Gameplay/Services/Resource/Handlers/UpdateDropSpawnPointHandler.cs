using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class UpdateDropSpawnPointHandler : ICommandHandler<UpdateDropSpawnPointCommand, bool>
  {
    private readonly IProgressWriter _progressWriter;
    private readonly ILogService _logService;

    public UpdateDropSpawnPointHandler(IProgressWriter progressWriter, ILogService logService)
    {
      _progressWriter = progressWriter;
      _logService = logService;
    }

    public bool Execute(in UpdateDropSpawnPointCommand command)
    {
      if (!_progressWriter.GameStateModel.WriteOnlyResourceDrops.TryGetValue(command.Id,
            out IResourceDropWriter resourceDrop))
      {
        _logService.LogError(GetType(),
          $"Resource drop with ID '{command.Id}' was not found in the current game state.", new KeyNotFoundException());
        return false;
      }

      resourceDrop.SpawnPoint.Value = command.Position;
      return true;
    }
  }
}