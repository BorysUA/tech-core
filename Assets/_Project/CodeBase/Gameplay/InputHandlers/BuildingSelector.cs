using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.UI.HUD;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.UI.Services;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class BuildingSelector : PlayerInputHandler
  {
    private readonly IGridService _gridService;
    private readonly CoordinateMapper _coordinateMapper;
    private readonly IBuildingService _buildingService;
    private readonly IGridOccupancyService _gridOccupancyService;
    private readonly HudViewModel _hudViewModel;

    public BuildingViewModel CurrentBuilding { get; private set; }

    public BuildingSelector(IGridService gridService, CoordinateMapper coordinateMapper,
      IBuildingService buildingService, IGridOccupancyService gridOccupancyService, HudViewModel hudViewModel)
    {
      _gridService = gridService;
      _coordinateMapper = coordinateMapper;
      _buildingService = buildingService;
      _gridOccupancyService = gridOccupancyService;
      _hudViewModel = hudViewModel;
    }

    public override void OnTap(Vector2 inputPoint)
    {
      CurrentBuilding?.Unselect();
      _hudViewModel.HideBuildingActionPanel();

      Vector3 worldPosition = _coordinateMapper.ScreenToWorldPoint(inputPoint);
      Vector3 snappedPosition = _gridService.GetSnappedPosition(worldPosition);
      Vector2Int cell = _gridService.GetCell(snappedPosition);

      if (_gridOccupancyService.TryGetCell(cell, out ICellStatus cellStatus) &&
          cellStatus.HasContent(CellContentType.Building))
      {
        BuildingViewModel buildingViewModel = _buildingService.GetBuildingById(cellStatus.BuildingId);
        CurrentBuilding = buildingViewModel;
        CurrentBuilding.Select();
        _hudViewModel.ShowBuildingActionPanel(CurrentBuilding);
      }
    }

    public override void OnDeactivated()
    {
      CurrentBuilding?.Unselect();
      _hudViewModel.HideBuildingActionPanel();
    }
  }
}