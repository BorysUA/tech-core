using System;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.Modules.Resource
{
  public class ResourceProductionModule : BuildingModule
  {
    private readonly IResourceService _resourceService;
    private readonly IProductionModifierService _productionModifierService;

    private ResourceFlowConfig _flowConfig;
    private DisposableBag _activationScope;

    public ResourceProductionModule(IResourceService resourceService,
      IProductionModifierService productionModifierService)
    {
      _resourceService = resourceService;
      _productionModifierService = productionModifierService;
    }

    public void Setup(ResourceFlowConfig resourceFlowConfig) =>
      _flowConfig = resourceFlowConfig;

    protected override void Activate()
    {
      Observable.Interval(TimeSpan.FromSeconds(_flowConfig.TickInterval))
        .Subscribe(_ => Produce())
        .AddTo(ref _activationScope);
    }

    protected override void Deactivate()
    {
      _activationScope.Clear();
    }

    public override void Dispose()
    {
      base.Dispose();
      _activationScope.Dispose();
    }

    private void Produce()
    {
      ResourceKind resource = _flowConfig.ResourceConfig.Kind;
      float productionBonus = _productionModifierService.GetMultiplier(resource);
      int total = Mathf.RoundToInt(_flowConfig.AmountPerTick * productionBonus);

      _resourceService.AddResource(resource, total);
    }
  }
}