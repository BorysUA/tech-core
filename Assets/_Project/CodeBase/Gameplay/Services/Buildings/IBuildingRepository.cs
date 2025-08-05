using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Building;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public interface IBuildingRepository
  {
    IEnumerable<BuildingViewModel> GetAll { get; }
    Observable<BuildingViewModel> BuildingsAdded { get; }
    Observable<BuildingViewModel> BuildingsRemoved { get; }
    BuildingViewModel GetBuildingById(int id);
  }
}