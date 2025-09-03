using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using R3;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.BuildingAction
{
  public class BuildingActionBarViewModel : IGameplayInit
  {
    private const int MaxSlots = 5;

    private readonly IStaticDataProvider _staticDataProvider;
    private readonly Dictionary<ActionType, IBuildingAction> _actionSlots = new(MaxSlots);
    private IReadOnlyList<ActionType> _orderedTypes = Array.Empty<ActionType>();

    public InitPhase InitPhase => InitPhase.Preparation;
    public event Action Showed;
    public event Action Hidden;

    public BuildingActionBarViewModel(IStaticDataProvider staticDataProvider)
    {
      _staticDataProvider = staticDataProvider;
    }

    public void Initialize()
    {
      _orderedTypes = _staticDataProvider.GetActionButtonsOrder();

      foreach (ActionType actionType in _orderedTypes)
        _actionSlots.Add(actionType, null);
    }

    public void Show(IBuildingActionReader buildingViewModel)
    {
      foreach (IBuildingActionsProvider actionsProvider in buildingViewModel.Actions)
      foreach (IBuildingAction buildingAction in actionsProvider.Actions)
        _actionSlots[buildingAction.Type] = buildingAction;

      Showed?.Invoke();
    }

    public void Hide()
    {
      Hidden?.Invoke();

      foreach (ActionType actionType in _orderedTypes)
        _actionSlots[actionType] = null;
    }

    public IEnumerable<IBuildingAction> GetActionsInOrder()
    {
      foreach (ActionType actionType in _orderedTypes)
      {
        IBuildingAction buildingAction = _actionSlots[actionType];

        if (buildingAction != null)
          yield return buildingAction;
      }
    }
  }
}