using _Project.CodeBase.Data.Progress.Building.ModuleData;

namespace _Project.CodeBase.Gameplay.Buildings.Modules
{
  public interface IProgressModule
  {
    void AttachData(IModuleData moduleData);
  }
}