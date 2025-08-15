using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Services.LogService;

namespace _Project.CodeBase.Gameplay.Buildings.Modules
{
  public abstract class BuildingModuleWithProgressData<TData> : BuildingModule, IProgressModule
  {
    protected readonly ILogService LogService;
    protected TData ModuleData { get; private set; }

    protected BuildingModuleWithProgressData(ILogService logService)
    {
      LogService = logService;
    }

    public virtual void AttachData(IModuleData moduleData)
    {
      if (moduleData is TData data)
        ModuleData = data;
      else
        LogService.LogError(GetType(), "Invalid Module data cast", new InvalidCastException());
    }
  }
}