using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Buildings.Conditions
{
  public class OperationalConditionTracer : IDisposable
  {
    private readonly CompositeDisposable _disposable = new();
    private readonly List<OperationalCondition> _conditions = new();
    private readonly ReactiveProperty<bool> _allSatisfied = new(true);

    public ReadOnlyReactiveProperty<bool> AllSatisfied => _allSatisfied;
    public IEnumerable<IBuildingIndicatorSource> Indicators => _conditions.Select(condition => condition.Indicator);

    public void Initialize()
    {
      foreach (OperationalCondition condition in _conditions)
        condition.Initialize();

      Observable
        .CombineLatest(_conditions
          .Select(c => c.IsSatisfied))
        .Select(values => values.All(x => x))
        .Subscribe(value => _allSatisfied.OnNext(value))
        .AddTo(_disposable);
    }

    public void AddConditions(IEnumerable<OperationalCondition> conditions)
    {
      foreach (OperationalCondition condition in conditions)
        _conditions.Add(condition);
    }

    public void Dispose()
    {
      foreach (OperationalCondition condition in _conditions)
        condition.Dispose();

      _disposable.Dispose();
    }
  }
}