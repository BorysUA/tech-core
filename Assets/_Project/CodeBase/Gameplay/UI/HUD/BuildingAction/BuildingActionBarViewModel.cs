using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Buildings;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD.BuildingAction
{
  public class BuildingActionBarViewModel
  {
    private readonly List<IBuildingAction> _buildingActions = new();
    private DisposableBag _disposable;

    public IEnumerable<IBuildingAction> BuildingActions => _buildingActions;

    public event Action Showed;
    public event Action Hidden;

    public void Show(IBuildingActionReader buildingViewModel)
    {
      foreach (IBuildingActionsProvider actionsProvider in buildingViewModel.Actions)
        _buildingActions.AddRange(actionsProvider.Actions);

      buildingViewModel.Destroyed
        .Subscribe(_ => Hide())
        .AddTo(ref _disposable);

      Showed?.Invoke();
    }

    public void Hide()
    {
      Hidden?.Invoke();
      _disposable.Clear();
      _buildingActions.Clear();
    }
  }
}