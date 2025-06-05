using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules.Resource
{
  public class ResourceConsumerModule : BuildingModule, IConditionBoundModule
  {
    private ResourceFlowConfig _resourceFlowConfig;
    private readonly IResourceService _resourceService;
    private OperationalConditionTracer _conditionTracer;
    private readonly List<IBuildingIndicatorSource> _indicators = new();
    private DisposableBag _disposableBag;

    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;
    public ReadOnlyReactiveProperty<bool> IsOperational => _conditionTracer.AllSatisfied;


    public ResourceConsumerModule(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    public void Setup(ResourceFlowConfig resourceFlowConfig) =>
      _resourceFlowConfig = resourceFlowConfig;

    public void Setup(List<OperationalCondition> conditions)
    {
      _conditionTracer = new OperationalConditionTracer(conditions);
      _indicators.AddRange(conditions);
    }

    public override void Initialize() =>
      _conditionTracer.Initialize();

    public override void Activate()
    {
      Observable.Interval(TimeSpan.FromSeconds(_resourceFlowConfig.TickInterval))
        .Subscribe(_ =>
        {
          _resourceService.TrySpend(_resourceFlowConfig.ResourceConfig.Kind, _resourceFlowConfig.AmountPerTick);
        })
        .AddTo(ref _disposableBag);
    }

    public override void Deactivate()
    {
      _disposableBag.Clear();
    }

    public override void OnDestroyed()
    {
      _disposableBag.Dispose();
    }
  }
}