namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  public interface IReadOnlyHealthData : IModuleData
  {
    public int Health { get; }
  }
}