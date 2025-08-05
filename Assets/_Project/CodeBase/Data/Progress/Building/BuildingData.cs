using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.Progress.Building
{
  [Serializable]
  public class BuildingData
  {
    public int Id;
    public BuildingType Type;
    public int Level;
    public Dictionary<Type, IModuleData> ModulesData;
    public Vector2Int[] OccupiedCells;

    public BuildingData(int id, BuildingType type, int level, Dictionary<Type, IModuleData> modulesData,
      Vector2Int[] occupiedCells)
    {
      Id = id;
      Type = type;
      Level = level;
      OccupiedCells = occupiedCells;
      ModulesData = modulesData;
    }
  }
}