using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Services.InputService;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class BuildingSelector : PlayerInputHandler
  {
    private readonly CoordinateMapper _coordinateMapper;
    private readonly IBuildingService _buildingService;
    private readonly IGridOccupancyService _gridOccupancyService;

    public BuildingSelector(CoordinateMapper coordinateMapper, IBuildingService buildingService,
      IGridOccupancyService gridOccupancyService)
    {
      _coordinateMapper = coordinateMapper;
      _buildingService = buildingService;
      _gridOccupancyService = gridOccupancyService;
    }

    public override void OnTap(Vector2 inputPoint)
    {
      Vector3 worldPosition = _coordinateMapper.ScreenToWorldPoint(inputPoint);
      Vector3 snappedPosition = GridUtils.GetSnappedPosition(worldPosition);
      Vector2Int cell = GridUtils.GetCell(snappedPosition);

      if (_gridOccupancyService.TryGetCell(cell, out ICellStatus status) && status.HasContent(CellContentType.Building))
        _buildingService.SelectBuilding(status.BuildingId);
      else
        _buildingService.UnselectCurrent();
    }

    public override void OnDeactivated()
    {
      _buildingService.UnselectCurrent();
    }
  }
}