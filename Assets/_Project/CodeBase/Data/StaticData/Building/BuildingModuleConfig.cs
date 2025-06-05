using System;
using _Project.CodeBase.Gameplay.Building.Modules;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building
{
  public abstract class BuildingModuleConfig : ScriptableObject
  {
    public abstract BuildingModule CreateBuildingModule(Func<Type, BuildingModule> instantiator,
      BuildingConfig buildingConfig);
  }
}