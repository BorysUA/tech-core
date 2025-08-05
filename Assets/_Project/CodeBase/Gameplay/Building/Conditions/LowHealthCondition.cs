using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class LowHealthCondition : PermanentCondition
  {
    private float _healthThreshold;
    private ReadOnlyReactiveProperty<float> _healthRatio;

    public void Setup(float healthThreshold, ReadOnlyReactiveProperty<float> healthRatio)
    {
      _healthThreshold = healthThreshold;
      _healthRatio = healthRatio;
    }

    protected override Observable<bool> BuildPermanentCondition() =>
      _healthRatio.Select(ratio => ratio > _healthThreshold);
  }
}