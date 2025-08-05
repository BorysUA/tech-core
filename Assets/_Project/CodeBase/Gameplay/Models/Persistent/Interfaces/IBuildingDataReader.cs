using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IBuildingDataReader
  {
    public int Id { get; }
    public BuildingType Type { get; }
    public ReadOnlyReactiveProperty<int> Level { get; }
    public IReadOnlyObservableDictionary<Type, IModuleData> ModulesProgress { get; }
    public IReadOnlyObservableList<Vector2Int> OccupiedCells { get; }
  }
}