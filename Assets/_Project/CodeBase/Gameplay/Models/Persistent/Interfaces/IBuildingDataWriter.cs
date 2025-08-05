using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IBuildingDataWriter : IBuildingDataReader
  {
    public new ReactiveProperty<int> Level { get; }
    public new ObservableDictionary<Type, IModuleData> ModulesProgress { get; }
    public new ObservableList<Vector2Int> OccupiedCells { get; }
  }
}