using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Constants;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.CodeBase.Data.Progress.Building
{
  [Serializable]
  public class BuildingData
  {
    public string Id;
    public BuildingType Type;
    public int Level;
    public Dictionary<Type, IModuleData> ModulesData = new();
    public List<Vector2Int> OccupiedCells;

    public BuildingData(string id, BuildingType type, int level, List<Vector2Int> occupiedCells)
    {
      Id = id;
      Type = type;
      Level = level;
      OccupiedCells = occupiedCells;
    }
  }
}