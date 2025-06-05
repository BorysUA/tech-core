using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.Modules.Health
{
  public class HealthModule : BuildingModuleWithProgressData<HealthData>, IDamageable, IBuildingActionsProvider,
    IConditionBoundModule
  {
    private HealthConfig _healthConfig;
    private DisposableBag _disposable;
    private readonly ActionFactory _actionFactory;
    private readonly IBuildingService _buildingService;
    private OperationalConditionTracer _conditionTracer;
    private readonly ReactiveProperty<int> _current = new();
    private readonly ReactiveProperty<float> _ratio = new();
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly List<IBuildingIndicatorSource> _indicators = new();

    public ReadOnlyReactiveProperty<int> Current => _current;
    public ReadOnlyReactiveProperty<float> Ratio => _ratio;
    public ReadOnlyReactiveProperty<bool> IsOperational => _conditionTracer.AllSatisfied;
    public IEnumerable<IBuildingAction> Actions => _actions;
    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;

    public HealthModule(ILogService logService, ActionFactory actionFactory, IBuildingService buildingService) :
      base(logService)
    {
      _actionFactory = actionFactory;
      _buildingService = buildingService;
    }

    public void Setup(HealthConfig healthConfig) =>
      _healthConfig = healthConfig;

    public void Setup(List<OperationalCondition> conditions)
    {
      _conditionTracer = new OperationalConditionTracer(conditions);
      _indicators.AddRange(conditions);
    }

    public override void Initialize()
    {
      _current.Value = Mathf.Clamp(ModuleData.Health, _healthConfig.Min, _healthConfig.Max);
      
      _current
        .Subscribe(health => ModuleData.Health = health)
        .AddTo(ref _disposable);

      Current.Select(health => (float)health / _healthConfig.Max)
        .Subscribe(ratio => _ratio.Value = ratio)
        .AddTo(ref _disposable);

      CreateActions();
      CreateIndicators();

      _conditionTracer.Initialize();
    }

    public override IModuleData CreateInitialData(string buildingId)
    {
      HealthData healthData = new HealthData(buildingId, _healthConfig.Max);
      return healthData;
    }

    public void TakeDamage(int damage)
    {
      _current.Value = Mathf.Max(_current.CurrentValue - damage, _healthConfig.Min);

      if (_current.CurrentValue == _healthConfig.Min)
      {
        Destroy();
      }
    }

    public override void OnDestroyed()
    {
      _disposable.Dispose();
      _conditionTracer.Dispose();

      foreach (IBuildingIndicatorSource indicator in _indicators)
        if (indicator is IDisposable indicatorDisposable)
          indicatorDisposable.Dispose();
    }

    private void Destroy()
    {
      _buildingService.DestroyBuilding(BuildingId);
    }

    private void CreateIndicators()
    {
      _indicators.Add(new DamagedIndicator(BuildingIndicatorType.Damaged, .99f, Ratio));
    }

    private void CreateActions()
    {
      RepairAction repairAction = _actionFactory.CreateAction<RepairAction>();
      repairAction.Setup(this);

      _actions[0] = repairAction;
    }
  }
}