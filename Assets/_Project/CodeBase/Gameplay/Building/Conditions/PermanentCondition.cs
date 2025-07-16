using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public abstract class PermanentCondition : OperationalCondition
  {
    protected abstract Observable<bool> BuildPermanentCondition();

    protected override (Observable<bool>, Observable<bool>) CreateStreams()
    {
      Observable<bool> stream = BuildPermanentCondition();
      return (stream, stream);
    }
  }
}