using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class EnoughResourceCondition : OperationalCondition
  {
    private ResourceAmountData _requiredResource;
    private BuildingIndicatorType _indicatorType;

    private readonly IResourceService _resourceService;

    public override BuildingIndicatorType IndicatorType => _indicatorType;

    public EnoughResourceCondition(IResourceService resourceService) =>
      _resourceService = resourceService;

    public void Setup(BuildingIndicatorType indicatorType, ResourceAmountData resourceAmountData)
    {
      _indicatorType = indicatorType;
      _requiredResource = resourceAmountData;
    }

    public override void Initialize()
    {
      base.Initialize();

      ReadOnlyReactiveProperty<int> observeResource = _resourceService.ObserveResource(_requiredResource.Kind);

      observeResource
        .Subscribe(amount => IsSatisfiedSource.Value = amount > _requiredResource.Amount)
        .AddTo(ref Disposable);
    }
  }
}