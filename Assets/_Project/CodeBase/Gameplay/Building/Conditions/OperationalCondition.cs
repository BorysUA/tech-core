using System;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
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
      (Observable<bool> activationCondition, Observable<bool> sustainCondition) = CreateStreams();

      IsSatisfied = _moduleIsOperational
        .Select(isActive => isActive ? activationCondition : sustainCondition)
        .Switch()
        .ToReadOnlyReactiveProperty()
        .AddTo(_subscriptions);

      Indicator = new ConditionFailureIndicator(_indicatorType, IsSatisfied);
    }

    protected abstract (Observable<bool> activationCondition, Observable<bool> sustainCondition) CreateStreams();

    public void Dispose()
    {
      _subscriptions.Dispose();
    }
  }
}