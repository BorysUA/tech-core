using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public interface IBuildingFactory
  {
    void Initialize();
    UniTask<BuildingPreview> CreateBuildingPreview(BuildingType buildingType);

    UniTask<BuildingViewModel> CreateBuilding(BuildingType buildingType, Vector3 position);
  }
}