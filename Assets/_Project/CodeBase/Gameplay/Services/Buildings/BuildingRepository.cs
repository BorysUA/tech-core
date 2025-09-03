using System;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using R3;
using Vector3 = UnityEngine.Vector3;

namespace _Project.CodeBase.Gameplay.Services.Buildings
{
  public class BuildingRepository : IBuildingRepository, IGameplayInit, IDisposable
  {
    private readonly IBuildingFactory _buildingFactory;
    private readonly IProgressReader _progressService;
    private readonly ILogService _logService;

    private readonly CompositeDisposable _subscriptions = new();

    private readonly Subject<BuildingViewModel> _buildingAdded = new();
    private readonly Subject<BuildingViewModel> _buildingRemoved = new();

    private readonly Dictionary<int, BuildingViewModel> _currentBuildings = new();

    public IEnumerable<BuildingViewModel> GetAll => _currentBuildings.Values;
    public Observable<BuildingViewModel> BuildingsAdded => _buildingAdded;
    public Observable<BuildingViewModel> BuildingsRemoved => _buildingRemoved;
    public InitPhase InitPhase => InitPhase.Creation;

    public BuildingRepository(IProgressReader progressService, IBuildingFactory buildingFactory, ILogService logService)
    {
      _progressService = progressService;
      _buildingFactory = buildingFactory;
      _logService = logService;
    }

    public void Initialize()
    {
      foreach (var buildingEntity in _progressService.GameStateModel.ReadOnlyBuildings.Values)
        CreateView(buildingEntity).Forget();

      _progressService.GameStateModel.ReadOnlyBuildings
        .ObserveAdd()
        .Subscribe(addEvent => CreateView(addEvent.Value).Forget())
        .AddTo(_subscriptions);

      _progressService.GameStateModel.ReadOnlyBuildings
        .ObserveRemove()
        .Subscribe(removeEvent => DestroyView(removeEvent.Value))
        .AddTo(_subscriptions);
    }

    public BuildingViewModel GetBuildingById(int id)
    {
      if (_currentBuildings.TryGetValue(id, out BuildingViewModel viewModel))
        return viewModel;

      _logService.LogError(GetType(), $"Building with ID '{id}' not found in '{nameof(_currentBuildings)} ");
      return null;
    }

    public void Dispose()
    {
      _subscriptions?.Dispose();

      foreach (BuildingViewModel viewModel in _currentBuildings.Values)
        viewModel.Dispose();
    }

    private async UniTaskVoid CreateView(IBuildingDataReader buildingData)
    {
      Vector3 worldBuildingPosition = GridUtils.GetWorldPivot(buildingData.OccupiedCells);

      BuildingViewModel viewModel = await _buildingFactory.CreateBuilding(buildingData.Type, worldBuildingPosition);
      viewModel.Initialize(buildingData);

      _currentBuildings.Add(buildingData.Id, viewModel);
      _buildingAdded.OnNext(viewModel);
    }

    private void DestroyView(IBuildingDataReader buildingData)
    {
      if (_currentBuildings.Remove(buildingData.Id, out BuildingViewModel viewModel))
      {
        _buildingRemoved.OnNext(viewModel);
        viewModel.Destroy();
      }
    }
  }
}