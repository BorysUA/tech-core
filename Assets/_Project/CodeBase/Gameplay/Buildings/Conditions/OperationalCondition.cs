using System;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators;
using R3;

namespace _Project.CodeBase.Gameplay.Buildings.Conditions
{
  public abstract class OperationalCondition : IDisposable
  {
    private readonly CompositeDisposable _subscriptions = new();

    private ReadOnlyReactiveProperty<bool> _moduleIsOperational;
    private BuildingIndicatorType _indicatorType;

    public ReadOnlyReactiveProperty<bool> IsSatisfied { get; private set; }
    public IBuildingIndicatorSource Indicator { get; private set; }

    public void Setup(ReadOnlyReactiveProperty<bool> moduleIsOperational, BuildingIndicatorType indicatorType)
    {
      _moduleIsOperational = moduleIsOperational;
      _indicatorType = indicatorType;
    }

    public void Initialize()
    {
      (Observable<bool> sustainCondition, Observable<bool> activationCondition) = CreateStreams();

      IsSatisfied = _moduleIsOperational
        .Select(isActive => isActive ? sustainCondition : activationCondition)
        .Switch()
        .ToReadOnlyReactiveProperty()
        .AddTo(_subscriptions);

      Indicator = new ConditionFailureIndicator(_indicatorType, IsSatisfied);
    }

    protected abstract (Observable<bool> sustainCondition, Observable<bool> activationCondition) CreateStreams();

    public virtual void Dispose()
    {
      _subscriptions.Dispose();
    }
  }
}