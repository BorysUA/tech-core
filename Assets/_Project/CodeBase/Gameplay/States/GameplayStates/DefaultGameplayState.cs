using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.States.GameplayStates.Placement;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Services.InputService;
using Zenject;

namespace _Project.CodeBase.Gameplay.States.GameplayStates
{
  public class DefaultGameplayState : IEnterState, IExitState
  {
    private readonly SignalBus _signalBus;
    private readonly GameplayStateMachine _gameplayStateMachine;
    private readonly ResourceCollector _resourceCollector;
    private readonly CameraMovement _cameraMovement;
    private readonly BuildingSelector _buildingSelector;
    private readonly IInputService _inputService;

    public DefaultGameplayState(SignalBus signalBus, GameplayStateMachine gameplayStateMachine,
      IInputService inputService, ResourceCollector resourceCollector, BuildingSelector buildingSelector,
      CameraMovement cameraMovement)
    {
      _signalBus = signalBus;
      _gameplayStateMachine = gameplayStateMachine;
      _inputService = inputService;
      _resourceCollector = resourceCollector;
      _buildingSelector = buildingSelector;
      _cameraMovement = cameraMovement;
    }

    public void Enter()
    {
      _inputService.SubscribeWithUiFilter(_resourceCollector);
      _inputService.SubscribeWithUiFilter(_buildingSelector);
      _inputService.SubscribeWithUiFilter(_cameraMovement);

      _signalBus.Subscribe<BuildingPurchaseRequested>(EnterPlaceBuildingState);
      _signalBus.Subscribe<ConstructionPlotPurchaseRequested>(EnterPlaceConstructionPlotState);
    }

    public void Exit()
    {
      _inputService.Unsubscribe(_resourceCollector);
      _inputService.Unsubscribe(_buildingSelector);
      _inputService.Unsubscribe(_cameraMovement);

      _signalBus.Unsubscribe<BuildingPurchaseRequested>(EnterPlaceBuildingState);
      _signalBus.Unsubscribe<ConstructionPlotPurchaseRequested>(EnterPlaceConstructionPlotState);
    }

    private void EnterPlaceBuildingState(BuildingPurchaseRequested request) =>
      _gameplayStateMachine.Enter<PlaceBuildingState, BuildingType>(request.Type);

    private void EnterPlaceConstructionPlotState(ConstructionPlotPurchaseRequested request) =>
      _gameplayStateMachine.Enter<PlaceConstructionPlotState, ConstructionPlotType>(request.Type);
  }
}