using System;
using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus.Indicators
{
  public class DamagedIndicator : IBuildingIndicatorSource, IDisposable
  {
    private readonly ReactiveProperty<bool> _isShown = new(false);
    private DisposableBag _disposable;
    public BuildingIndicatorType IndicatorType { get; }
    public ReadOnlyReactiveProperty<bool> IsShown => _isShown;

    public DamagedIndicator(BuildingIndicatorType buildingIndicatorType, float healthThreshold,
      ReadOnlyReactiveProperty<float> healthRatio)
    {
      IndicatorType = buildingIndicatorType;

      healthRatio
        .Subscribe(ratio => _isShown.Value = ratio < healthThreshold)
        .AddTo(ref _disposable);
    }

    public void Dispose() =>
      _disposable.Dispose();
  }
}