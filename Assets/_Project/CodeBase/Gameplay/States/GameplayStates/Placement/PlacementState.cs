using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.States.GameplayStates.Placement
{
  public abstract class PlacementState<T> : IPayloadState<T>, IExitState
  {
    protected readonly GridPlacement GridPlacement;
    private readonly IInputService _inputService;
    private readonly IPopUpService _popUpService;

    protected PlacementState(GridPlacement gridPlacement, IInputService inputService, IPopUpService popUpService)
    {
      GridPlacement = gridPlacement;
      _inputService = inputService;
      _popUpService = popUpService;
    }

    public virtual void Enter(T type)
    {
      InternalEnterAsync(type).Forget();
    }

    private async UniTaskVoid InternalEnterAsync(T type)
    {
      await SetupPlacement(type);
      _inputService.SubscribeWithUiFilter(GridPlacement);
      _popUpService.ShowPopUp<ConfirmPlacePopUp, ConfirmPlaceViewModel>();
      RunPlacement().Forget();
    }

    public virtual void Exit()
    {
      _inputService.Unsubscribe(GridPlacement);
    }

    protected abstract UniTask SetupPlacement(T type);

    protected abstract void ProcessResult(PlacementResult placementResult);

    protected abstract bool IsPlacementValid(IEnumerable<Vector2Int> placeCells);

    private async UniTaskVoid RunPlacement()
    {
      PlacementResult placementResult = await GridPlacement.ExecutePlacementAsync();

      ProcessResult(placementResult);
    }
  }
}