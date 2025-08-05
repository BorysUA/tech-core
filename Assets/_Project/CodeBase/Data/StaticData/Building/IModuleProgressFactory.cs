using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;

namespace _Project.CodeBase.Data.StaticData.Building
{
  public interface IModuleProgressFactory
  {
    public (Type moduleType, IModuleData data) CreateInitialData();
  }
}