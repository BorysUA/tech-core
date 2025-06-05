using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public abstract class OperationalCondition : IBuildingIndicatorSource
  {
    protected DisposableBag Disposable;
    protected readonly ReactiveProperty<bool> IsSatisfiedSource = new();

    public abstract BuildingIndicatorType IndicatorType { get; }
    public ReadOnlyReactiveProperty<bool> IsShown { get; private set; }
    public ReadOnlyReactiveProperty<bool> IsSatisfied => IsSatisfiedSource;

    public virtual void Initialize()
    {
      IsShown = IsSatisfied
        .Select(satisfied => !satisfied)
        .ToReadOnlyReactiveProperty()
        .AddTo(ref Disposable);
    }

    public virtual void Dispose()
    {
      Disposable.Dispose();
    }
  }
}