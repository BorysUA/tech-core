using System;
using System.Collections.Generic;
using System.Linq;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class OperationalConditionTracer : IDisposable
  {
    private readonly CompositeDisposable _disposable = new();
    private readonly List<OperationalCondition> _conditions;
    private readonly ReactiveProperty<bool> _allSatisfied = new(true);

    public ReadOnlyReactiveProperty<bool> AllSatisfied => _allSatisfied;

    public OperationalConditionTracer(List<OperationalCondition> conditions) =>
      _conditions = conditions;

    public void Initialize()
    {
      foreach (OperationalCondition condition in _conditions) 
        condition.Initialize();
      
      Observable<bool> allConditionsObservable = Observable
        .CombineLatest(_conditions
          .Select(c => c.IsSatisfied))
        .Select(values => values.All(x => x));

      allConditionsObservable
        .Subscribe(value => _allSatisfied.OnNext(value))
        .AddTo(_disposable);
    }

    public void Dispose()
    {
      foreach (OperationalCondition condition in _conditions)
        condition.Dispose();

      _disposable.Dispose();
    }
  }
}