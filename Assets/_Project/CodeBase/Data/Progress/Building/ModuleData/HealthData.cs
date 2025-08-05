using System;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class HealthData : IReadOnlyHealthData
  {
    public int Health { get; set; }

    public HealthData(int health)
    {
      Health = health;
    }
  }
}