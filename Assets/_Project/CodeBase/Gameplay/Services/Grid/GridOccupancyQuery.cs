using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Markers.Baked;
using _Project.CodeBase.Gameplay.Markers.Baked.Payloads;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Infrastructure.StateMachine;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public class GridOccupancyQuery : IGridOccupancyQuery, IDisposable, IGameplayInit
  {
    private readonly IProgressReader _progressService;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly CompositeDisposable _disposable = new();

    private Dictionary<Vector2Int, CellData> OccupiedCells { get; } = new();
    public InitPhase InitPhase => InitPhase.Preparation;

    public GridOccupancyQuery(IProgressReader progressService, IStaticDataProvider staticDataProvider)
    {
      _progressService = progressService;
      _staticDataProvider = staticDataProvider;
    }

    public void Initialize()
    {
      FillOccupiedCellsFromStaticData();
      FillOccupiedCellsFromProgress();
      ObserveConstructionPlotsCollection();
      ObserveBuildingsCollection();
    }

    public CellContentType GetCellContentMask(Vector2Int position) =>
      OccupiedCells.TryGetValue(position, out CellData cellData)
        ? cellData.ContentMask
        : CellContentType.None;

    public CellStatus GetCellOrEmpty(Vector2Int position)
    {
      if (OccupiedCells.TryGetValue(position, out CellData cellData))
      {
        return new CellStatus
        {
          BuildingId = cellData.BuildingId,
          Mask = cellData.ContentMask,
          PlotId = cellData.ConstructionPlotId,
          ResourceSpotKind = cellData.ResourceSpotKind,
        };
      }

      return CellStatus.None;
    }

    public bool TryGetCell(Vector2Int position, out CellStatus cell)
    {
      cell = default;

      if (OccupiedCells.TryGetValue(position, out CellData cellData))
      {
        cell = new CellStatus()
        {
          BuildingId = cellData.BuildingId,
          Mask = cellData.ContentMask,
          PlotId = cellData.ConstructionPlotId,
          ResourceSpotKind = cellData.ResourceSpotKind,
        };
        return true;
      }

      return false;
    }

    public bool DoesCellMatchFilter(Vector2Int cellPosition, PlacementFilter filter)
    {
      CellContentType contentMask = GetCellContentMask(cellPosition);

      if ((contentMask & filter.MustHave) != filter.MustHave)
        return false;

      if ((contentMask & filter.MustBeEmpty) != 0)
        return false;

      return true;
    }

    public bool DoesCellsMatchFilter(IEnumerable<Vector2Int> cellsPosition, PlacementFilter filter) =>
      cellsPosition.All(position => DoesCellMatchFilter(position, filter));

    public void Dispose()
    {
      _disposable?.Dispose();
    }

    private void FillOccupiedCellsFromProgress()
    {
      foreach (var constructionPlot in _progressService.GameStateModel.ReadOnlyPlots.Values)
        RegisterConstructionPlotInGrid(constructionPlot);

      foreach (var buildingEntry in _progressService.GameStateModel.ReadOnlyBuildings.Values)
        RegisterBuildingInGrid(buildingEntry);
    }

    private void FillOccupiedCellsFromStaticData()
    {
      IEnumerable<MapEntityData> resourceSpots = _staticDataProvider
        .GetMapEntities()
        .Where(entity => entity.Type is MapEntityType.ResourceSpot);

      foreach (MapEntityData entity in resourceSpots)
        RegisterResourceSpotInGrid(((ResourceSpotData)entity.Payload).Kind, entity.OccupiedCells);

      IEnumerable<MapEntityData> obstacles = _staticDataProvider
        .GetMapEntities()
        .Where(entity => entity.Type is MapEntityType.Obstacle);

      foreach (MapEntityData entity in obstacles)
        RegisterObstacleInGrid(entity.OccupiedCells);
    }

    private void ObserveBuildingsCollection()
    {
      _progressService.GameStateModel.ReadOnlyBuildings
        .ObserveAdd()
        .Subscribe(addEvent => RegisterBuildingInGrid(addEvent.Value))
        .AddTo(_disposable);

      _progressService.GameStateModel.ReadOnlyBuildings
        .ObserveRemove()
        .Subscribe(removeEvent => UnregisterBuildingInGrid(removeEvent.Value))
        .AddTo(_disposable);
    }

    private void ObserveConstructionPlotsCollection()
    {
      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveAdd()
        .Subscribe(addEvent => { RegisterConstructionPlotInGrid(addEvent.Value); })
        .AddTo(_disposable);

      _progressService.GameStateModel.ReadOnlyPlots
        .ObserveRemove()
        .Subscribe(removeEvent => { UnregisterConstructionPlotInGrid(removeEvent.Value); })
        .AddTo(_disposable);
    }

    private CellData GetOrCreate(Vector2Int position)
    {
      if (OccupiedCells.TryGetValue(position, out CellData cellData))
        return cellData;

      cellData = new CellData();
      OccupiedCells.Add(position, cellData);
      return cellData;
    }

    private void RegisterResourceSpotInGrid(ResourceKind kind, IEnumerable<Vector2Int> occupiedCells)
    {
      foreach (Vector2Int cellPosition in occupiedCells)
        GetOrCreate(cellPosition).SetResourceSpot(kind);
    }

    private void RegisterObstacleInGrid(List<Vector2Int> occupiedCells)
    {
      foreach (Vector2Int cellPosition in occupiedCells)
        GetOrCreate(cellPosition).SetObstacle();
    }

    private void RegisterConstructionPlotInGrid(IPlotDataReader proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
        GetOrCreate(cellPosition).SetConstructionPlot(proxy.Id);
    }

    private void UnregisterConstructionPlotInGrid(IPlotDataReader proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
      {
        OccupiedCells[cellPosition].RemoveConstructionPlot();

        if (OccupiedCells[cellPosition].HasContent(CellContentType.None))
          OccupiedCells.Remove(cellPosition);
      }
    }

    private void RegisterBuildingInGrid(IBuildingDataReader proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
        GetOrCreate(cellPosition).SetBuilding(proxy.Id);
    }

    private void UnregisterBuildingInGrid(IBuildingDataReader proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
      {
        OccupiedCells[cellPosition].RemoveBuilding();

        if (OccupiedCells[cellPosition].HasContent(CellContentType.None))
          OccupiedCells.Remove(cellPosition);
      }
    }
  }
}