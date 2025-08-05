using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public abstract class SwitchableCondition : OperationalCondition
  {
    protected abstract Observable<bool> BuildSustainCondition();
    protected abstract Observable<bool> BuildActivationCondition();

    protected override (Observable<bool>, Observable<bool>) CreateStreams()
      => (BuildSustainCondition(), BuildActivationCondition());
  }
}