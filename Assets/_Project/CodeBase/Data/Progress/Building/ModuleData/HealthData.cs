using System;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class HealthData : IModuleData
  {
    public string BuildingId { get; }
    public int Health { get; set; }

    public HealthData(string id, int health)
    {
      BuildingId = id;
      Health = health;
    }
  }
}