using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class BuildingService : IBuildingService, IGameplayInit, IDisposable
  {
    private readonly ICommandBroker _commandBroker;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IGameplayPhaseFlow _gameplayPhaseFlow;
    private readonly CompositeDisposable _subscriptions = new();

    private readonly ReactiveProperty<int?> _currentSelectedBuilding = new(null);
    private readonly Dictionary<BuildingCategory, IEnumerable<BuildingInfo>> _availableSortedBuildings = new();

    public IReadOnlyDictionary<BuildingCategory, IEnumerable<BuildingInfo>> AvailableSortedBuildings =>
      _availableSortedBuildings;

    public ReadOnlyReactiveProperty<int?> CurrentSelectedBuilding => _currentSelectedBuilding;
    public InitPhase InitPhase => InitPhase.Preparation;

    public BuildingService(ICommandBroker commandBroker, IStaticDataProvider staticDataProvider,
      IBuildingRepository buildingRepository, IGameplayPhaseFlow gameplayPhaseFlow)
    {
      _commandBroker = commandBroker;
      _staticDataProvider = staticDataProvider;
      _buildingRepository = buildingRepository;
      _gameplayPhaseFlow = gameplayPhaseFlow;
    }

    public void Initialize()
    {
      foreach (var categoryGroup in _staticDataProvider.GetBuildingsShopCatalog().Categories)
      {
        _availableSortedBuildings.Add(categoryGroup.Category, categoryGroup.Buildings.Select(buildingType =>
        {
          BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(buildingType);
          return new BuildingInfo(buildingType, buildingConfig.Category, buildingConfig.SizeInCells);
        }).ToList());
      }

      foreach (BuildingViewModel viewModel in _buildingRepository.GetAll)
        _gameplayPhaseFlow.Register(viewModel);

      _buildingRepository.BuildingsAdded
        .Subscribe(viewModel => _gameplayPhaseFlow.Register(viewModel))
        .AddTo(_subscriptions);

      _buildingRepository.BuildingsRemoved
        .Subscribe(viewModel =>
        {
          _gameplayPhaseFlow.Unregister(viewModel);

          if (_currentSelectedBuilding.CurrentValue == viewModel.Id)
            _currentSelectedBuilding.Value = null;
        })
        .AddTo(_subscriptions);
    }

    public void PlaceBuilding(BuildingType buildingType, List<Vector2Int> position)
    {
      PlaceBuildingCommand placeBuildingCommand = new PlaceBuildingCommand(buildingType, 1, position);

      _commandBroker.ExecuteCommand(placeBuildingCommand);
    }

    public void DestroyBuilding(int buildingId)
    {
      DestroyBuildingCommand destroyBuildingCommand = new DestroyBuildingCommand(buildingId);
      _commandBroker.ExecuteCommand(destroyBuildingCommand);
    }

    public IBuildingActionReader GetActionsForBuilding(int buildingId) =>
      _buildingRepository.GetBuildingById(buildingId);

    public void SelectBuilding(int id)
    {
      UnselectCurrent();
      BuildingViewModel selectedBuilding = _buildingRepository.GetBuildingById(id);

      if (selectedBuilding == null)
        return;

      selectedBuilding.Select();
      _currentSelectedBuilding.OnNext(id);
    }

    public void UnselectCurrent()
    {
      if (_currentSelectedBuilding.CurrentValue == null)
        return;

      BuildingViewModel selectedBuilding =
        _buildingRepository.GetBuildingById(_currentSelectedBuilding.CurrentValue.Value);

      selectedBuilding.Unselect();
      _currentSelectedBuilding.OnNext(null);
    }

    public void Dispose() =>
      _subscriptions?.Dispose();
  }
}