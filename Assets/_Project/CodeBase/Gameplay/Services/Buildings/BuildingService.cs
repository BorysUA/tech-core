using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class BuildingService : IBuildingService, IGameplayInit
  {
    private readonly ICommandBroker _commandBroker;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IBuildingRepository _buildingRepository;
    private readonly IGameplayPhaseFlow _gameplayPhaseFlow;
    private readonly CompositeDisposable _subscriptions = new();

    private readonly ReactiveProperty<BuildingViewModel> _currentSelectedBuilding = new(null);
    private readonly Dictionary<BuildingCategory, IEnumerable<BuildingInfo>> _availableSortedBuildings = new();

    public IReadOnlyDictionary<BuildingCategory, IEnumerable<BuildingInfo>> AvailableSortedBuildings =>
      _availableSortedBuildings;

    public ReadOnlyReactiveProperty<IBuildingActionReader> CurrentSelectedBuilding { get; private set; }

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
      CurrentSelectedBuilding = _currentSelectedBuilding
        .Select(viewModel => (IBuildingActionReader)viewModel)
        .ToReadOnlyReactiveProperty()
        .AddTo(_subscriptions);

      foreach (var categoryGroup in _staticDataProvider.GetBuildingsShopCatalog().Categories)
      {
        _availableSortedBuildings.Add(categoryGroup.Category, categoryGroup.Buildings.Select(buildingType =>
        {
          BuildingConfig buildingConfig = _staticDataProvider.GetBuildingConfig(buildingType);
          return new BuildingInfo(buildingType, buildingConfig.Category, buildingConfig.SizeInCells);
        }).ToList());
      }

      _buildingRepository.BuildingsAdded
        .Subscribe(viewModel => _gameplayPhaseFlow.Register(viewModel))
        .AddTo(_subscriptions);

      _buildingRepository.BuildingsRemoved
        .Subscribe(viewModel => _gameplayPhaseFlow.Unregister(viewModel))
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

    public void SelectBuilding(int id)
    {
      UnselectCurrent();
      BuildingViewModel selectedBuilding = _buildingRepository.GetBuildingById(id);

      if (selectedBuilding == null)
        return;

      selectedBuilding.Select();
      _currentSelectedBuilding.OnNext(selectedBuilding);
    }

    public void UnselectCurrent()
    {
      if (_currentSelectedBuilding.CurrentValue == null)
        return;

      _currentSelectedBuilding.CurrentValue.Unselect();
      _currentSelectedBuilding.OnNext(null);
    }
  }
}