using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using ObservableCollections;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public interface IBuildingService
  {
    IObservableCollection<BuildingType> AvailableBuildings { get; }
    void PlaceBuilding(BuildingType buildingType, List<Vector2Int> position);
    BuildingViewModel GetBuildingById(string id);
    void DestroyBuilding(string buildingId);
  }
}