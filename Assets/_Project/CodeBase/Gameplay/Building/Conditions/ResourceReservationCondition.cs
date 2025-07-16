using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Services.LogService;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class ResourceReservationCondition : SwitchableCondition
  {
    private readonly IResourceService _resourceService;
    private readonly ILogService _logService;

    private readonly Subject<bool> _isReserved = new();

    private ResourceAmountData _requiredResource;
    private ReservationToken _reservationToken;

    public ResourceReservationCondition(IResourceService resourceService, ILogService logService)
    {
      _resourceService = resourceService;
      _logService = logService;
    }

    public void Setup(ResourceAmountData resourceAmountData)
    {
      _requiredResource = resourceAmountData;
    }

    protected override Observable<bool> BuildActivationCondition()
    {
      return Observable.Defer(() =>
      {
        if (_resourceService.TryReserve(_requiredResource.Kind, _requiredResource.Amount,
              out ReservationToken reservationToken))
        {
          _isReserved.OnNext(true);
          _reservationToken = reservationToken;
          _reservationToken.Canceled += OnReservationCanceled;
        }
        else
        {
          _isReserved.OnNext(false);
          _logService.LogWarning(GetType(), $"Failed to reserve resource {_requiredResource.Kind}");
        }

        return _isReserved;
      });
    }

    protected override Observable<bool> BuildSustainCondition()
    {
      return Observable.Defer(() =>
      {
        _reservationToken?.Cancel();

        return _resourceService
          .ObserveResource(_requiredResource.Kind)
          .Select(amount => amount >= _requiredResource.Amount);
      });
    }

    private void OnReservationCanceled()
    {
      _reservationToken.Canceled -= OnReservationCanceled;
      _isReserved.OnNext(false);
    }
  }
}