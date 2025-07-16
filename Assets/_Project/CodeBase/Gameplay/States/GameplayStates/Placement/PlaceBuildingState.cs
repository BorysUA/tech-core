using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.States.GameplayStates.Placement
{
  public class PlaceBuildingState : PlacementState<BuildingType>
  {
    private readonly IBuildingService _buildingService;
    private readonly IBuildingFactory _buildingFactory;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly IGridOccupancyService _gridOccupancyService;
    private readonly CoordinateMapper _coordinateMapper;

    private PlacementFilter _placementFilter;
    private BuildingType _buildingType;

    public PlaceBuildingState(IPopUpService popUpService, IInputService inputService, IBuildingService buildingService,
      GridPlacement gridPlacement, IStaticDataProvider staticDataProvider,
      GameplayStateMachine gameplayStateMachine, IBuildingFactory buildingFactory,
      IGridOccupancyService gridOccupancyService, CoordinateMapper coordinateMapper) : base(gridPlacement,
      inputService, popUpService)
    {
      _buildingService = buildingService;
      _staticDataProvider = staticDataProvider;
      _gameplayStateMachine = gameplayStateMachine;
      _buildingFactory = buildingFactory;
      _gridOccupancyService = gridOccupancyService;
      _coordinateMapper = coordinateMapper;
    }

    protected override async UniTask SetupPlacement(BuildingType type)
    {
      _buildingType = type;

      BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(type);
      _placementFilter = buildingConfig.PlacementFilter;

      BuildingPreview preview = await _buildingFactory.CreateBuildingPreview(type);
      Vector3 defaultPosition = _coordinateMapper.CenterScreenToWorldPoint();

      GridPlacement.Setup(preview, defaultPosition, buildingConfig.SizeInCells, IsPlacementValid);
    }

    protected override bool IsPlacementValid(IEnumerable<Vector2Int> placeCells)
    {
      foreach (Vector2Int cell in placeCells)
      {
        CellContentType contentMask = _gridOccupancyService.GetCellContentMask(cell);

        if (!DoesCellMatchFilter(contentMask, _placementFilter))
          return false;
      }

      return true;
    }

    protected override void ProcessResult(PlacementResult placementResult)
    {
      if (placementResult.IsConfirmed)
        _buildingService.PlaceBuilding(_buildingType, placementResult.Cells);

      _gameplayStateMachine.Enter<DefaultGameplayState>();
    }
  }
}