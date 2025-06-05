using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Resource.Behaviours;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Services.LogService;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules.Resource
{
  public class ResourceReservationModule : BuildingModule, IConditionBoundModule
  {
    private readonly IResourceService _resourceService;
    private ResourceAmountData _resourceToReserve;
    private ReservationToken _reservationToken;
    private readonly ILogService _logService;
    private OperationalConditionTracer _conditionTracer;
    private readonly ReactiveProperty<bool> _isReserved = new();
    private readonly List<IBuildingIndicatorSource> _indicators = new();

    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;

    public ReadOnlyReactiveProperty<bool> IsReserved => _isReserved;
    public ReadOnlyReactiveProperty<bool> IsOperational => _conditionTracer.AllSatisfied;

    public ResourceReservationModule(IResourceService resourceService, ILogService logService)
    {
      _resourceService = resourceService;
      _logService = logService;
    }

    public void Setup(List<OperationalCondition> conditions)
    {
      _conditionTracer = new OperationalConditionTracer(conditions);
      _indicators.AddRange(conditions);
    }

    public void Setup(ResourceAmountData resourceAmountData) =>
      _resourceToReserve = resourceAmountData;

    public override void Initialize()
    {
      _conditionTracer.Initialize();
    }

    public override void Activate()
    {
      base.Activate();

      if (_resourceService.TryReserve(_resourceToReserve.Kind, _resourceToReserve.Amount,
            out ReservationToken reservationToken))
      {
        _isReserved.Value = true;
        _reservationToken = reservationToken;
        _reservationToken.Canceled += OnReservationCanceled;
      }
      else
      {
        _logService.LogWarning(GetType(), $"Failed to reserve resource {_resourceToReserve.Kind}");
      }
    }

    public override void Deactivate()
    {
      _reservationToken?.Cancel();
      _isReserved.Value = false;
    }

    private void OnReservationCanceled()
    {
      _isReserved.Value = false;
      _reservationToken = null;
    }
  }
}