using System;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Data
{
  public class BuildingDataProxy
  {
    public string Id { get; }
    public BuildingType Type { get; }
    public ReactiveProperty<int> Level { get; }
    public ObservableDictionary<Type, IModuleData> ModulesProgress { get; }
    public ObservableList<Vector2Int> OccupiedCells { get; }

    public BuildingData Origin { get; }

    public BuildingDataProxy(BuildingData buildingData)
    {
      Origin = buildingData;

      Id = buildingData.Id;
      Type = buildingData.Type;

      ModulesProgress = new ObservableDictionary<Type, IModuleData>(buildingData.ModulesData);
      Level = new ReactiveProperty<int>(buildingData.Level);
      OccupiedCells = new ObservableList<Vector2Int>(buildingData.OccupiedCells);

      ModulesProgress.ObserveAdd()
        .Subscribe(addEvent => buildingData.ModulesData.Add(addEvent.Value.Key, addEvent.Value.Value));

      ModulesProgress.ObserveRemove()
        .Subscribe(removeEvent => buildingData.ModulesData.Remove(removeEvent.Value.Key));

      Level.Subscribe(value => buildingData.Level = value);

      OccupiedCells.ObserveReplace()
        .Subscribe(replaceEvent => buildingData.OccupiedCells[replaceEvent.Index] = replaceEvent.NewValue);
    }
  }
}