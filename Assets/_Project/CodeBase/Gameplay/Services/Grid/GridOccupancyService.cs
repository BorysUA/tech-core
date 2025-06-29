using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Grid
{
  public class GridOccupancyService : IGridOccupancyService, IDisposable, IGameplayInit
  {
    private readonly IProgressService _progressService;

    private readonly IStaticDataProvider _staticDataProvider;

    private readonly CompositeDisposable _disposable = new();

    private Dictionary<Vector2Int, CellData> OccupiedCells { get; } = new();

    public GridOccupancyService(IProgressService progressService, IStaticDataProvider staticDataProvider)
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

    public bool TryGetCell(Vector2Int position, out ICellStatus cellStatus)
    {
      if (OccupiedCells.TryGetValue(position, out CellData cellData))
      {
        cellStatus = cellData;
        return true;
      }

      cellStatus = null;
      return false;
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }

    private void FillOccupiedCellsFromProgress()
    {
      foreach (var constructionPlot in _progressService.GameStateProxy.ConstructionPlotsCollection)
        RegisterConstructionPlotInGrid(constructionPlot);

      foreach (var buildingEntry in _progressService.GameStateProxy.BuildingsCollection)
        RegisterBuildingInGrid(buildingEntry.Value);
    }

    private void FillOccupiedCellsFromStaticData()
    {
      foreach (ResourceSpotEntry spot in _staticDataProvider.GetResourceSpots())
        RegisterResourceSpotInGrid(spot);
    }

    private void ObserveBuildingsCollection()
    {
      _progressService.GameStateProxy.BuildingsCollection
        .ObserveAdd()
        .Subscribe(addEvent => RegisterBuildingInGrid(addEvent.Value.Value))
        .AddTo(_disposable);

      _progressService.GameStateProxy.BuildingsCollection
        .ObserveRemove()
        .Subscribe(removeEvent => UnregisterBuildingInGrid(removeEvent.Value.Value))
        .AddTo(_disposable);
    }

    private void ObserveConstructionPlotsCollection()
    {
      _progressService.GameStateProxy.ConstructionPlotsCollection
        .ObserveAdd()
        .Subscribe(addEvent => { RegisterConstructionPlotInGrid(addEvent.Value); })
        .AddTo(_disposable);

      _progressService.GameStateProxy.ConstructionPlotsCollection
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

    private void RegisterResourceSpotInGrid(ResourceSpotEntry spot)
    {
      foreach (Vector2Int cellPosition in spot.OccupiedCells)
        GetOrCreate(cellPosition).SetResourceSpot(spot.Kind);
    }

    private void RegisterConstructionPlotInGrid(ConstructionPlotDataProxy proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
        GetOrCreate(cellPosition).SetConstructionPlot(proxy.Id);
    }

    private void UnregisterConstructionPlotInGrid(ConstructionPlotDataProxy proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
      {
        OccupiedCells[cellPosition].RemoveConstructionPlot();

        if (OccupiedCells[cellPosition].HasContent(CellContentType.None))
          OccupiedCells.Remove(cellPosition);
      }
    }

    private void RegisterBuildingInGrid(BuildingDataProxy proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
        OccupiedCells[cellPosition].SetBuilding(proxy.Id);
    }

    private void UnregisterBuildingInGrid(BuildingDataProxy proxy)
    {
      foreach (Vector2Int cellPosition in proxy.OccupiedCells)
        OccupiedCells[cellPosition].RemoveBuilding();
    }
  }
}