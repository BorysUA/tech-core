using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public abstract class BuildingModule : IDisposable
  {
    private readonly CompositeDisposable _subscriptions = new();
    private readonly OperationalConditionTracer _localConditionsTracer = new();
    private readonly OperationalConditionTracer _globalConditionsTracer = new();
    private readonly List<IBuildingIndicatorSource> _indicators = new();
    private readonly ReactiveProperty<bool> _isModuleWorking = new();

    private bool _ignoreGlobalConditions;

    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;
    public ReadOnlyReactiveProperty<bool> IsModuleWorking => _isModuleWorking;
    public ReadOnlyReactiveProperty<bool> CanBuildingWork => _globalConditionsTracer.AllSatisfied;
    protected string BuildingId { get; private set; }

    public void Setup(bool ignoreGlobalConditions)
    {
      _ignoreGlobalConditions = ignoreGlobalConditions;
    }

    public void RegisterConditions(List<OperationalCondition> local, List<OperationalCondition> global)
    {
      _localConditionsTracer.AddConditions(local);
      _globalConditionsTracer.AddConditions(global);
    }

    public void Initialize(string buildingId, Observable<bool> buildingOperational)
    {
      BuildingId = buildingId;

      _localConditionsTracer.Initialize();
      _globalConditionsTracer.Initialize();

      _indicators.AddRange(_localConditionsTracer.Indicators);
      _indicators.AddRange(_globalConditionsTracer.Indicators);

      Observable<bool> buildingOperationalConditions =
        _ignoreGlobalConditions ? Observable.Return(true) : buildingOperational;

      OnInitialize();

      Observable.CombineLatest(
          _localConditionsTracer.AllSatisfied,
          buildingOperationalConditions,
          (canModuleRun, canBuildingRun) => canModuleRun && canBuildingRun)
        .DistinctUntilChanged()
        .Subscribe(OnActiveStateChanged)
        .AddTo(_subscriptions);
    }

    protected virtual void OnInitialize()
    {
    }

    public virtual void OnSelected()
    {
    }

    public virtual void OnUnselected()
    {
    }

    protected virtual void Activate()
    {
    }

    protected virtual void Deactivate()
    {
    }

    public virtual void Dispose()
    {
      _subscriptions?.Dispose();

      foreach (IBuildingIndicatorSource indicator in _indicators)
        if (indicator is IDisposable disposable)
          disposable.Dispose();
    }

    protected void GuardActive([CallerMemberName] string method = null)
    {
      if (!_isModuleWorking.CurrentValue)
        throw new InvalidOperationException($"[{GetType().Name}.{method}] called when module is inactive.");
    }

    protected void AddIndicator(IBuildingIndicatorSource indicator) =>
      _indicators.Add(indicator);

    private void OnActiveStateChanged(bool isActive)
    {
      _isModuleWorking.Value = isActive;

      if (isActive)
        Activate();
      else
        Deactivate();
    }
  }
}