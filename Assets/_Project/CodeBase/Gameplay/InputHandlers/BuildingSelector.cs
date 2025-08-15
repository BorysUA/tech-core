using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Services.InputService;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _Project.CodeBase.Gameplay.InputHandlers
{
  public class BuildingSelector : PlayerInputHandler
  {
    private readonly CoordinateMapper _coordinateMapper;
    private readonly IBuildingService _buildingService;
    private readonly IGridOccupancyQuery _gridOccupancyQuery;

    public BuildingSelector(CoordinateMapper coordinateMapper, IBuildingService buildingService,
      IGridOccupancyQuery gridOccupancyQuery)
    {
      _coordinateMapper = coordinateMapper;
      _buildingService = buildingService;
      _gridOccupancyQuery = gridOccupancyQuery;
    }

    public override void OnTap(Vector2 inputPoint)
    {
      Vector3 worldPosition = _coordinateMapper.ScreenToWorldPoint(inputPoint);
      Vector3 snappedPosition = GridUtils.GetSnappedPosition(worldPosition);
      Vector2Int cell = GridUtils.GetCell(snappedPosition);

      CellStatus cellStatus = _gridOccupancyQuery.GetCellOrEmpty(cell);

      if (!cellStatus.IsEmpty && cellStatus.HasContent(CellContentType.Building))
        _buildingService.SelectBuilding(cellStatus.BuildingId);
      else
        _buildingService.UnselectCurrent();
    }

    public override void OnDeactivated()
    {
      _buildingService.UnselectCurrent();
    }
  }
}