using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.UI.Factory;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.BuildingAction
{
  public class BuildingActionBar : MonoBehaviour, IDisposable
  {
    [SerializeField] private Transform _buttonsContainer;

    private BuildingActionBarViewModel _barViewModel;
    private readonly List<BuildingActionButton> _actionButtons = new();
    private IGameplayUiFactory _gameplayUiFactory;
    private DisposableBag _disposable;

    [Inject]
    public void Setup(IGameplayUiFactory gameplayUiFactory, BuildingActionBarViewModel barViewModel)
    {
      _gameplayUiFactory = gameplayUiFactory;
      _barViewModel = barViewModel;

      _barViewModel.Showed += Show;
      _barViewModel.Hidden += Hide;
    }

    private async void Show()
    {
      foreach (IBuildingAction action in _barViewModel.BuildingActions)
      {
        await CreateActionButton(action);
      }

      gameObject.SetActive(true);
    }

    private void Hide()
    {
      gameObject.SetActive(false);

      foreach (BuildingActionButton actionButton in _actionButtons)
        actionButton.Deactivate();

      _actionButtons.Clear();
      _disposable.Clear();
    }

    public void Dispose()
    {
      _barViewModel.Showed -= Show;
      _barViewModel.Hidden -= Hide;
    }

    private async UniTask CreateActionButton(IBuildingAction action)
    {
      BuildingActionButton button = await _gameplayUiFactory.CreateBuildingActionButton(action.Type, _buttonsContainer);

      button.OnClick
        .Subscribe(_ => action.Execute())
        .AddTo(ref _disposable);

      _actionButtons.Add(button);
    }
  }
}