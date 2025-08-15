using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using _Project.CodeBase.Gameplay.Services.Resource;
using R3;

namespace _Project.CodeBase.Gameplay.Buildings.Conditions
{
  public class ResourceReservationCondition : SwitchableCondition
  {
    private readonly IResourceService _resourceService;

    private readonly ReactiveProperty<bool> _isReserved = new();

    private ResourceAmountData _requiredResource;
    private ReservationToken _reservationToken;

    public ResourceReservationCondition(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    public void Setup(ResourceAmountData resourceAmountData)
    {
      _requiredResource = resourceAmountData;
    }

    protected override Observable<bool> BuildSustainCondition()
    {
      return Observable.Defer(() => _isReserved);
    }

    protected override Observable<bool> BuildActivationCondition()
    {
      return Observable.Defer(() =>
      {
        _reservationToken?.Cancel();

        return _resourceService
          .ObserveResource(_requiredResource.Kind)
          .Select(amount =>
          {
            if (_reservationToken is { IsActive: true })
              return true;

            if (amount >= _requiredResource.Amount)
            {
              if (_resourceService.TryReserve(_requiredResource.Kind, _requiredResource.Amount,
                    out ReservationToken reservationToken))
              {
                _isReserved.OnNext(true);
                _reservationToken = reservationToken;
                _reservationToken.Canceled += OnReservationCanceled;
                return true;
              }
            }

            return false;
          });
      });
    }

    public override void Dispose()
    {
      base.Dispose();
      _reservationToken?.Cancel();
    }

    private void OnReservationCanceled()
    {
      _reservationToken.Canceled -= OnReservationCanceled;
      _isReserved.OnNext(false);
    }
  }
}