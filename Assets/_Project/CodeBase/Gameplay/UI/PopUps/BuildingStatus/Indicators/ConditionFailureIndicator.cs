using System;
using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators
{
  public class ConditionFailureIndicator : IBuildingIndicatorSource, IDisposable
  {
    private readonly ReactiveProperty<bool> _isVisible = new(false);
    private readonly CompositeDisposable _disposables = new();
    public BuildingIndicatorType Type { get; }
    public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

    public ConditionFailureIndicator(BuildingIndicatorType type, ReadOnlyReactiveProperty<bool> conditionStatus)
    {
      Type = type;

      conditionStatus
        .Select(isConditionSatisfied => !isConditionSatisfied)
        .Subscribe(shouldShow => _isVisible.Value = shouldShow)
        .AddTo(_disposables);
    }

    public void Dispose() =>
      _disposables.Dispose();
  }
}