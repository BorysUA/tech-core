using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Services;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class BuildingService : IBuildingService, IDisposable
  {
    private readonly ICommandBroker _commandBroker;
    private readonly IBuildingFactory _buildingFactory;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IGridService _gridService;
    private readonly IResourceService _resourceService;
    private readonly ILogService _logService;
    private readonly IPopUpService _popUpService;
    private readonly CompositeDisposable _disposable = new();

    private readonly Dictionary<string, BuildingViewModel> _buildings = new();
    private readonly ObservableList<BuildingType> _availableBuildings = new();
    public IObservableCollection<BuildingType> AvailableBuildings => _availableBuildings;

    public BuildingService(ICommandBroker commandBroker, IProgressService progressService,
      IBuildingFactory buildingFactory, IStaticDataProvider staticDataProvider,
      IGridService gridService, IResourceService resourceService, ILogService logService, IPopUpService popUpService)
    {
      _commandBroker = commandBroker;
      _buildingFactory = buildingFactory;
      _staticDataProvider = staticDataProvider;
      _gridService = gridService;
      _resourceService = resourceService;
      _logService = logService;
      _popUpService = popUpService;

      foreach (var buildingEntity in progressService.GameStateProxy.BuildingsCollection)
        CreateView(buildingEntity.Value);

      foreach (BuildingConfig buildingConfig in _staticDataProvider.GetAllBuildings()) 
        _availableBuildings.Add(buildingConfig.Type);

      progressService.GameStateProxy.BuildingsCollection
        .ObserveAdd()
        .Subscribe(addEvent => CreateView(addEvent.Value.Value))
        .AddTo(_disposable);

      progressService.GameStateProxy.BuildingsCollection
        .ObserveRemove()
        .Subscribe(removeEvent => DestroyView(removeEvent.Value.Value))
        .AddTo(_disposable);
    }

    public void PlaceBuilding(BuildingType buildingType, List<Vector2Int> position)
    {
      BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(buildingType);

      if (_resourceService.TrySpend(buildingConfig.Price.Resource.Kind,
            buildingConfig.Price.Amount))
      {
        PlaceBuildingCommand placeBuildingCommand = new PlaceBuildingCommand(
          buildingType, buildingConfig.StartLevel, position);

        _commandBroker.ExecuteCommand(placeBuildingCommand);
      }
    }

    public void DestroyBuilding(string buildingId)
    {
      DestroyBuildingCommand destroyBuildingCommand = new DestroyBuildingCommand(buildingId);
      _commandBroker.ExecuteCommand(destroyBuildingCommand);
    }

    public BuildingViewModel GetBuildingById(string id)
    {
      if (_buildings.TryGetValue(id, out BuildingViewModel viewModel))
        return viewModel;

      _logService.LogError(GetType(), $"Building with ID '{id}' not found in '{nameof(_buildings)} ");
      return null;
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }

    private async void CreateView(BuildingDataProxy buildingData)
    {
      Vector3 worldBuildingPosition = _gridService.GetWorldPivot(buildingData.OccupiedCells);

      BuildingViewModel viewModel = await _buildingFactory.CreateBuilding(buildingData.Type, worldBuildingPosition);
      viewModel.Initialize(buildingData);

      _popUpService.ShowPopUp<BuildingIndicatorsPopUp, BuildingIndicatorsViewModel, BuildingViewModel>(viewModel,false);

      _buildings.Add(buildingData.Id, viewModel);
    }

    private void DestroyView(BuildingDataProxy buildingData)
    {
      if (_buildings.Remove(buildingData.Id, out BuildingViewModel buildingViewModel))
        buildingViewModel.Destroy();
    }
  }
}