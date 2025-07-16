using System;
using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators
{
  public class ThresholdIndicator<T> : IBuildingIndicatorSource, IDisposable
  {
    private readonly ReactiveProperty<bool> _isVisible = new(false);
    private readonly CompositeDisposable _disposable = new();
    public BuildingIndicatorType Type { get; }
    public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

    public ThresholdIndicator(BuildingIndicatorType type, Observable<T> source, T threshold,
      Func<T, T, bool> shouldShow)
    {
      Type = type;

      source
        .Select(current => shouldShow(current, threshold))
        .Subscribe(isVisible => _isVisible.Value = isVisible)
        .AddTo(_disposable);
    }

    public void Dispose() =>
      _disposable.Dispose();
  }
}