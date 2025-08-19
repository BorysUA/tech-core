using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class BuildingIndicatorsPopUp : BasePopUp<BuildingIndicatorsViewModel>
  {
    [SerializeField] private Transform _indicatorsContainer;
    [SerializeField] private RectTransform _rectTransform;

    private IGameplayUiFactory _uiFactory;

    private readonly Dictionary<BuildingIndicatorType, Task<BuildingIndicatorView>> _indicatorViewTasks = new();

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
        .Subscribe(indicator => OnIndicatorChanged(indicator).Forget())
        .AddTo(this);

      ViewModel.Initialized -= OnInitialize;
    }

    private void OnDestroyed()
    {
      ViewModel.Initialized -= OnInitialize;
      ViewModel.Destroyed -= OnDestroyed;

      Destroy(gameObject);
    }

    private async UniTaskVoid OnIndicatorChanged(IndicatorVisibility indicator)
    {
      BuildingIndicatorView view = await GetViewAsync(indicator.Type);
      view.SetVisible(indicator.Visible);
    }

    private void SetPosition(Vector2 position) =>
      _rectTransform.anchoredPosition = position;

    private Task<BuildingIndicatorView> GetViewAsync(BuildingIndicatorType type)
    {
      if (_indicatorViewTasks.TryGetValue(type, out Task<BuildingIndicatorView> existingTask))
        return existingTask;

      Task<BuildingIndicatorView> task = _uiFactory.CreateBuildingIndicator(type, _indicatorsContainer);
      _indicatorViewTasks[type] = task;
      return task;
    }
  }
}