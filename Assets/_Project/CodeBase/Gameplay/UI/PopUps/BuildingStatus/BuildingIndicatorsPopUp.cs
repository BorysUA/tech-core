using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.UI.Common;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class BuildingIndicatorsPopUp : BasePopUp<BuildingIndicatorsViewModel>
  {
    [SerializeField] private Transform _indicatorsContainer;
    [SerializeField] private RectTransform _rectTransform;

    private IGameplayUiFactory _uiFactory;
    private readonly Dictionary<BuildingIndicatorType, BuildingIndicatorView> _indicatorViews = new();

    [Inject]
    public void Construct(IGameplayUiFactory uiFactory) =>
      _uiFactory = uiFactory;

    public override void Setup(BuildingIndicatorsViewModel viewModel)
    {
      base.Setup(viewModel);

      viewModel.Initialized += OnInitialize;
      viewModel.Destroyed += OnDestroyed;
    }

    private void OnInitialize()
    {
      ViewModel.Position
        .Subscribe(SetPosition)
        .AddTo(this);

      ViewModel.IndicatorChanged
        .Subscribe(OnIndicatorChanged)
        .AddTo(this);

      ViewModel.Initialized -= OnInitialize;
    }

    private void OnDestroyed()
    {
      ViewModel.Initialized -= OnInitialize;
      ViewModel.Destroyed -= OnDestroyed;

      Destroy(gameObject);
    }

    private async void OnIndicatorChanged(IndicatorVisibility indicator)
    {
      if (!_indicatorViews.TryGetValue(indicator.Type, out BuildingIndicatorView indicatorView))
      {
        indicatorView = await _uiFactory.CreateBuildingIndicator(indicator.Type, _indicatorsContainer);

        _indicatorViews.TryAdd(indicator.Type, indicatorView);
      }

      indicatorView.SetVisible(indicator.Visible);
    }

    private void SetPosition(Vector2 position) =>
      _rectTransform.anchoredPosition = position;
  }
}