using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Services.Resource;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Conditions
{
  public class SpendPerTickCondition : SwitchableCondition
  {
    private readonly IResourceService _resourceService;

    private ResourceAmountData _activationThreshold;
    private ResourceFlowConfig _flowConfig;

    public SpendPerTickCondition(IResourceService resourceService) =>
      _resourceService = resourceService;

    public void Setup(ResourceAmountData activationThreshold, ResourceFlowConfig flowConfig)
    {
      _activationThreshold = activationThreshold;
      _flowConfig = flowConfig;
    }

    protected override Observable<bool> BuildSustainCondition()
    {
      return Observable.Interval(TimeSpan.FromSeconds(_flowConfig.TickInterval))
        .Select(_ => _resourceService
          .TrySpend(_flowConfig.ResourceConfig.Kind, ResourceSink.Production, _flowConfig.AmountPerTick));
    }

    protected override Observable<bool> BuildActivationCondition()
    {
      return _resourceService
        .ObserveResource(_activationThreshold.Kind)
        .Select(amount => amount > _activationThreshold.Amount);
    }
  }
}