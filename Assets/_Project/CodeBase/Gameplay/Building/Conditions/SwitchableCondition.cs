using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public abstract class SwitchableCondition : OperationalCondition
  {
    protected abstract Observable<bool> BuildActivationCondition();
    protected abstract Observable<bool> BuildSustainCondition();

    protected override (Observable<bool>, Observable<bool>) CreateStreams()
      => (BuildActivationCondition(), BuildSustainCondition());
  }
}