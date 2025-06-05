using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class ResourceReservationCondition : OperationalCondition
  {
    private readonly IResourceService _resourceService;

    private BuildingIndicatorType _indicatorType;
    private ResourceAmountData _resourceToReserve;
    private DisposableBag _resourceObserverSubscription;
    private ReadOnlyReactiveProperty<bool> _isReserved;

    public override BuildingIndicatorType IndicatorType => _indicatorType;

    public ResourceReservationCondition(IResourceService resourceService) =>
      _resourceService = resourceService;

    public void Setup(BuildingIndicatorType indicatorType, ResourceAmountData resourceAmount,
      ReadOnlyReactiveProperty<bool> isReserved)
    {
      _indicatorType = indicatorType;
      _resourceToReserve = resourceAmount;
      _isReserved = isReserved;
    }

    public override void Initialize()
    {
      base.Initialize();

      ReadOnlyReactiveProperty<int> resourceObserver = _resourceService.ObserveResource(_resourceToReserve.Kind);

      _isReserved
        .Subscribe(isReserved =>
        {
          if (isReserved)
          {
            IsSatisfiedSource.Value = true;
            _resourceObserverSubscription.Clear();
          }
          else
            ObserveResource(resourceObserver);
        })
        .AddTo(ref Disposable);
    }

    private void ObserveResource(ReadOnlyReactiveProperty<int> resourceObserver)
    {
      resourceObserver
        .Subscribe(currentAmount => IsSatisfiedSource.Value = currentAmount >= _resourceToReserve.Amount)
        .AddTo(ref _resourceObserverSubscription);
    }

    public override void Dispose()
    {
      base.Dispose();
      _resourceObserverSubscription.Dispose();
    }
  }
}