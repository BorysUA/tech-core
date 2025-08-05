using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building.ModuleData;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public interface IProgressModule
  {
    void AttachData(IModuleData moduleData);
  }
}