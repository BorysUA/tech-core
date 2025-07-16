using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.Modules.Health
{
  public class HealthModule : BuildingModuleWithProgressData<HealthData>, IDamageable, IBuildingActionsProvider
  {
    private readonly ActionFactory _actionFactory;
    private readonly IBuildingService _buildingService;
    private readonly CompositeDisposable _lifetimeDisposables = new();
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly ReactiveProperty<int> _current = new();

    private HealthConfig _healthConfig;
    private DisposableBag _activationScope;

    public ReadOnlyReactiveProperty<int> Current => _current;
    public IEnumerable<IBuildingAction> Actions => _actions;
    public ReadOnlyReactiveProperty<float> Ratio { get; private set; }

    public HealthModule(ILogService logService, ActionFactory actionFactory, IBuildingService buildingService) :
      base(logService)
    {
      _actionFactory = actionFactory;
      _buildingService = buildingService;
    }

    public void Setup(HealthConfig healthConfig)
    {
      _healthConfig = healthConfig;

      Ratio = _current
        .Select(health => (float)health / _healthConfig.Max)
        .ToReadOnlyReactiveProperty(1f)
        .AddTo(_lifetimeDisposables);
    }

    public override IModuleData CreateInitialData(string buildingId)
    {
      HealthData healthData = new HealthData(buildingId, _healthConfig.Max);
      return healthData;
    }

    protected override void OnInitialize()
    {
      _current.Value = Mathf.Clamp(ModuleData.Health, _healthConfig.Min, _healthConfig.Max);

      CreateActions();
      CreateIndicators();
    }

    public void TakeDamage(int damage)
    {
      GuardActive();

      _current.Value = Mathf.Max(_current.CurrentValue - damage, _healthConfig.Min);

      if (_current.CurrentValue == _healthConfig.Min)
        Destroy();
    }

    protected override void Activate()
    {
      _current
        .Subscribe(health => ModuleData.Health = health)
        .AddTo(ref _activationScope);
    }

    protected override void Deactivate()
    {
      _activationScope.Clear();
    }

    public override void Dispose()
    {
      base.Dispose();
      _activationScope.Dispose();
      _lifetimeDisposables.Dispose();
    }

    private void Destroy()
    {
      _buildingService.DestroyBuilding(BuildingId);
    }

    private void CreateIndicators()
    {
      AddIndicator(new ThresholdIndicator<float>(BuildingIndicatorType.Damaged, Ratio, 0.99f,
        (current, threshold) => current < threshold));
    }

    private void CreateActions()
    {
      RepairAction repairAction = _actionFactory.CreateAction<RepairAction>();
      repairAction.Setup(this);

      _actions[0] = repairAction;
    }
  }
}