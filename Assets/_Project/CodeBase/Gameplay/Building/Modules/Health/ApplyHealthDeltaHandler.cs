using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Building.Modules.Health
{
  public class ApplyHealthDeltaHandler : ICommandHandler<ApplyHealthDeltaCommand, bool>
  {
    private readonly ModuleContextResolver _moduleContextResolver;

    public ApplyHealthDeltaHandler(ModuleContextResolver moduleContextResolver)
    {
      _moduleContextResolver = moduleContextResolver;
    }

    public bool Execute(in ApplyHealthDeltaCommand command)
    {
      if (command.Delta == 0)
        return false;

      ModuleContext<HealthModuleConfig, HealthData> moduleContext =
        _moduleContextResolver.Resolve<HealthModuleConfig, HealthData>(command.BuildingId);

      int oldHp = moduleContext.Data.Health;
      int newHp = Math.Clamp(moduleContext.Data.Health + command.Delta, moduleContext.Config.HealthConfig.Min,
        moduleContext.Config.HealthConfig.Max);

      if (newHp == oldHp)
        return false;

      moduleContext.Data.Health = newHp;
      return true;
    }
  }
}