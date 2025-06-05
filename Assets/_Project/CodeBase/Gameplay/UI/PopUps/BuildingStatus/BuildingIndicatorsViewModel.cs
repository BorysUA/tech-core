using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.UI.PopUps.Common;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.UI.Common;
using _Project.CodeBase.UI.Services;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class BuildingIndicatorsViewModel : BasePopUpViewModel, IParameterizedPopUp<BuildingViewModel>
  {
    private BuildingViewModel _buildingViewModel;

    private readonly ICameraMovement _cameraMovement;
    private readonly CoordinateMapper _coordinateMapper;
    private readonly IPopUpService _popUpService;
    private readonly IStaticDataProvider _staticDataProvider;

    private readonly CompositeDisposable _disposable = new();
    private readonly ReactiveProperty<Vector2> _position = new();

    private readonly ReplaySubject<IndicatorVisibility> _indicatorChanged = new();
    private readonly Dictionary<IndicatorSlotType, IndicatorSlot> _slots = new();

    public event Action Initialized;
    public event Action Destroyed;

    public ReadOnlyReactiveProperty<Vector2> Position => _position;
    public Observable<IndicatorVisibility> IndicatorChanged => _indicatorChanged;

    public BuildingIndicatorsViewModel(ICameraMovement cameraMovement, CoordinateMapper coordinateMapper,
      IPopUpService popUpService, IStaticDataProvider staticDataProvider)
    {
      _cameraMovement = cameraMovement;
      _coordinateMapper = coordinateMapper;
      _popUpService = popUpService;
      _staticDataProvider = staticDataProvider;
    }

    public void Initialize(BuildingViewModel viewModel)
    {
      _buildingViewModel = viewModel;

      InitializeIndicators(viewModel.Indicators);

      _buildingViewModel.Destroyed
        .Subscribe(_ => OnDestroy())
        .AddTo(_disposable);

      _cameraMovement.MovementDelta
        .Subscribe(UpdateIndicatorPanelPosition)
        .AddTo(_disposable);
      
      Initialized?.Invoke();
    }

    private void InitializeIndicators(IEnumerable<IBuildingIndicatorSource> indicators)
    {
      foreach (IBuildingIndicatorSource indicatorSource in indicators)
      {
        BuildingIndicatorConfig config = _staticDataProvider.GetBuildingIndicatorConfig(indicatorSource.IndicatorType);

        Indicator indicator =
          new Indicator(config.SlotType, indicatorSource.IndicatorType, indicatorSource.IsShown, config.Priority);

        if (!_slots.TryGetValue(indicator.SlotType, out IndicatorSlot slot))
          _slots[indicator.SlotType] = slot = new IndicatorSlot();

        slot.Indicators.Add(indicator);

        indicatorSource.IsShown
          .Subscribe(shown => OnIndicatorVisibilityChanged(indicator, shown))
          .AddTo(slot.Disposable);
      }
    }

    private void OnIndicatorVisibilityChanged(Indicator indicator, bool shown)
    {
      IndicatorSlot slot = _slots[indicator.SlotType];

      if (shown && indicator.Priority > (slot.ActiveIndicator?.Priority ?? int.MinValue))
        ActivateIndicator(slot, indicator);
      else if (!shown && indicator == slot.ActiveIndicator)
        PromoteNext(slot);
    }

    private void ActivateIndicator(IndicatorSlot slot, Indicator indicator)
    {
      if (slot.ActiveIndicator is not null)
        _indicatorChanged.OnNext(new IndicatorVisibility(slot.ActiveIndicator.Type, false));

      slot.ActiveIndicator = indicator;
      _indicatorChanged.OnNext(new IndicatorVisibility(indicator.Type, true));
    }

    private void PromoteNext(IndicatorSlot slot)
    {
      _indicatorChanged.OnNext(new IndicatorVisibility(slot.ActiveIndicator.Type, false));

      slot.ActiveIndicator = null;

      foreach (Indicator candidateIndicator in slot.Indicators.Where(indicator => indicator.IsShown.CurrentValue))
        if (candidateIndicator.Priority > (slot.ActiveIndicator?.Priority ?? int.MinValue))
          slot.ActiveIndicator = candidateIndicator;

      if (slot.ActiveIndicator is not null)
        _indicatorChanged.OnNext(new IndicatorVisibility(slot.ActiveIndicator.Type, true));
    }

    private void OnDestroy()
    {
      _disposable?.Dispose();

      foreach (IndicatorSlot slot in _slots.Values)
        slot.Disposable.Dispose();

      Destroyed?.Invoke();
    }

    private void UpdateIndicatorPanelPosition(Vector3 movementDelta)
    {
      Vector2 screenPoint = _coordinateMapper.WorldToScreenPoint(_buildingViewModel.WorldPosition);
      if (_popUpService.ScreenToCanvasPoint(screenPoint, out Vector2 localPoint))
        _position.Value = localPoint;
    }
  }
}