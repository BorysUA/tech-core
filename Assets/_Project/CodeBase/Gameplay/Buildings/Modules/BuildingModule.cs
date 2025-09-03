using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _Project.CodeBase.Gameplay.Buildings.Conditions;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.Modules
{
  public abstract class BuildingModule : IDisposable
  {
    private readonly CompositeDisposable _subscriptions = new();
    private readonly OperationalConditionTracer _localConditionsTracer = new();
    private readonly OperationalConditionTracer _globalConditionsTracer = new();
    private readonly List<IBuildingIndicatorSource> _indicators = new();
    private readonly ReactiveProperty<bool> _isModuleWorking = new();

    private bool _ignoreGlobalConditions;
    private Observable<bool> _buildingOperationalConditions;

    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;
    public ReadOnlyReactiveProperty<bool> IsModuleWorking => _isModuleWorking;
    public ReadOnlyReactiveProperty<bool> CanBuildingWork => _globalConditionsTracer.AllSatisfied;
    protected int BuildingId { get; private set; }

    public void Setup(bool ignoreGlobalConditions)
    {
      _ignoreGlobalConditions = ignoreGlobalConditions;
    }

    public void RegisterConditions(List<OperationalCondition> local, List<OperationalCondition> global)
    {
      _localConditionsTracer.AddConditions(local);
      _globalConditionsTracer.AddConditions(global);
    }

    public void Initialize(int buildingId, Observable<bool> buildingOperational)
    {
      BuildingId = buildingId;

      _buildingOperationalConditions = _ignoreGlobalConditions ? Observable.Return(true) : buildingOperational;

      OnInitialize();

      _localConditionsTracer.Initialize();
      _globalConditionsTracer.Initialize();

      _indicators.AddRange(_localConditionsTracer.Indicators);
      _indicators.AddRange(_globalConditionsTracer.Indicators);
    }

    public void Run()
    {
      Observable.CombineLatest(
          _localConditionsTracer.AllSatisfied,
          _buildingOperationalConditions,
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
      if (_isModuleWorking.CurrentValue)
        Deactivate();

      _localConditionsTracer.Dispose();
      _globalConditionsTracer.Dispose();

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
      if (isActive)
        Activate();
      else
        Deactivate();

      _isModuleWorking.OnNext(isActive);
    }
  }
}