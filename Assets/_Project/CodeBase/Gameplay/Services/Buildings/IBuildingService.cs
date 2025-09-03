using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public interface IBuildingService
  {
    IReadOnlyDictionary<BuildingCategory, IEnumerable<BuildingInfo>> AvailableSortedBuildings { get; }
    ReadOnlyReactiveProperty<int?> CurrentSelectedBuilding { get; }
    void PlaceBuilding(BuildingType buildingType, List<Vector2Int> position);
    void DestroyBuilding(int buildingId);
    void SelectBuilding(int id);
    void UnselectCurrent();
    IBuildingActionReader GetActionsForBuilding(int buildingId);
  }
}