using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.Modules.Health
{
  public class HealthModule : BuildingModuleWithProgressData<IReadOnlyHealthData>, IDamageable, IBuildingActionsProvider
  {
    private const float RatioDamagedThreshold = 0.99f;

    private readonly ICommandBroker _commandBroker;
    private readonly ActionFactory _actionFactory;
    private readonly IBuildingService _buildingService;
    private readonly CompositeDisposable _lifetimeDisposables = new();
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly ReactiveProperty<int> _current = new();

    private HealthConfig _healthConfig;

    public ReadOnlyReactiveProperty<int> Current => _current;
    public IReadOnlyList<IBuildingAction> Actions => _actions;
    public ReadOnlyReactiveProperty<float> Ratio { get; private set; }

    public HealthModule(ICommandBroker commandBroker, ILogService logService, ActionFactory actionFactory,
      IBuildingService buildingService) : base(logService)
    {
      _commandBroker = commandBroker;
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

    protected override void OnInitialize()
    {
      _current.Value = Mathf.Clamp(ModuleData.Health, _healthConfig.Min, _healthConfig.Max);

      CreateActions();
      CreateIndicators();
    }

    public void TakeDamage(int damage)
    {
      GuardActive();

      if (damage <= 0)
        return;

      bool isApplied = _commandBroker.ExecuteCommand<ApplyHealthDeltaCommand, bool>(
        new ApplyHealthDeltaCommand(BuildingId, -damage));

      if (!isApplied)
        return;

      _current.Value = ModuleData.Health;
      if (ModuleData.Health == _healthConfig.Min)
        DestroyBuilding();
    }

    public override void Dispose()
    {
      base.Dispose();
      _lifetimeDisposables.Dispose();
    }

    private void DestroyBuilding()
    {
      _buildingService.DestroyBuilding(BuildingId);
    }

    private void CreateIndicators()
    {
      AddIndicator(new ThresholdIndicator<float>(BuildingIndicatorType.Damaged, Ratio, RatioDamagedThreshold,
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