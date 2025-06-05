using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class LowHealthCondition : OperationalCondition
  {
    private float _healthThreshold;
    private ReadOnlyReactiveProperty<float> _healthRatio;

    public override BuildingIndicatorType IndicatorType => BuildingIndicatorType.CriticallyDamaged;

    public void Setup(float healthThreshold, ReadOnlyReactiveProperty<float> healthRatio)
    {
      _healthThreshold = healthThreshold;
      _healthRatio = healthRatio;
    }

    public override void Initialize()
    {
      base.Initialize();

      _healthRatio
        .Subscribe(ratio => IsSatisfiedSource.Value = ratio > _healthThreshold)
        .AddTo(ref Disposable);
    }
  }
}