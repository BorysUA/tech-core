using System;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Services.Resource;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules.Resource
{
  public class ResourceProductionModule : BuildingModule
  {
    private readonly IResourceService _resourceService;
    private ResourceFlowConfig _flowConfig;

    private DisposableBag _disposableBag;

    public ResourceProductionModule(IResourceService resourceService) =>
      _resourceService = resourceService;

    public void Setup(ResourceFlowConfig resourceFlowConfig) =>
      _flowConfig = resourceFlowConfig;

    public override void Activate()
    {
      Observable.Interval(TimeSpan.FromSeconds(_flowConfig.TickInterval))
        .Subscribe(_ => _resourceService.AddResource(_flowConfig.ResourceConfig.Kind, _flowConfig.AmountPerTick))
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