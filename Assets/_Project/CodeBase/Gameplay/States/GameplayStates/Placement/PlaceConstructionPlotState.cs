using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.States.GameplayStates.Placement
{
  public class PlaceConstructionPlotState : PlacementState<ConstructionPlotType>
  {
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IConstructionPlotFactory _constructionPlotFactory;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly IConstructionPlotService _constructionPlotService;
    private readonly IGridOccupancyService _gridOccupancyService;
    private readonly CoordinateMapper _coordinateMapper;

    private ConstructionPlotType _type;
    private PlacementFilter _placementFilter;

    public PlaceConstructionPlotState(GridPlacement gridPlacement, IInputService inputService,
      IPopUpService popUpService, IStaticDataProvider staticDataProvider,
      GameplayStateMachine gameplayStateMachine, IConstructionPlotFactory constructionPlotFactory,
      IConstructionPlotService constructionPlotService, IGridOccupancyService gridOccupancyService,
      CoordinateMapper coordinateMapper) : base(
      gridPlacement, inputService, popUpService)
    {
      _staticDataProvider = staticDataProvider;
      _gameplayStateMachine = gameplayStateMachine;
      _constructionPlotFactory = constructionPlotFactory;
      _constructionPlotService = constructionPlotService;
      _gridOccupancyService = gridOccupancyService;
      _coordinateMapper = coordinateMapper;
    }

    protected override async UniTask SetupPlacement(ConstructionPlotType type)
    {
      _type = type;

      Vector3 defaultPosition = _coordinateMapper.CenterScreenToWorldPoint();
      ConstructionPlotConfig plotConfig = _staticDataProvider.GetConstructionPlotConfig(type);
      ConstructionPlotPreview preview = await _constructionPlotFactory.CreateConstructionPlotPreview(type);
      _placementFilter = plotConfig.PlacementFilter;

      GridPlacement.Setup(preview, defaultPosition, plotConfig.SizeInCells, IsPlacementValid);
    }

    protected override bool IsPlacementValid(IEnumerable<Vector2Int> placeCells) =>
      _gridOccupancyService.DoesCellsMatchFilter(placeCells, _placementFilter);

    protected override void ProcessResult(PlacementResult placementResult)
    {
      if (placementResult.IsConfirmed)
        _constructionPlotService.PlaceConstructionPlot(_type, placementResult.Cells);

      _gameplayStateMachine.Enter<DefaultGameplayState>();
    }
  }
}