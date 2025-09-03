using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Constants;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public interface IBuildingFactory
  {
    UniTask<BuildingPreview> CreateBuildingPreview(BuildingType buildingType);
    UniTask<BuildingViewModel> CreateBuilding(BuildingType buildingType, Vector3 position);
  }
}