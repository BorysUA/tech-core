using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services.Resource;

namespace _Project.CodeBase.Gameplay.Building.Modules.Resource
{
  public class CapacityModule : BuildingModule
  {
    private readonly IResourceService _resourceService;
    private CapacityEffect _capacityEffect;
    
    public CapacityModule(IResourceService resourceService) =>
      _resourceService = resourceService;

    public void Setup(CapacityEffect capacityEffect) =>
      _capacityEffect = capacityEffect;

    protected override void Activate()
    {
      ResourceAmountData resource = _capacityEffect.AdditionalCapacity;

      if (_resourceService.IncreaseCapacity(resource.Kind, resource.Amount) && _capacityEffect.FillOnAdd)
        _resourceService.AddResource(resource.Kind, resource.Amount);
    }

    protected override void Deactivate()
    {
      ResourceAmountData resource = _capacityEffect.AdditionalCapacity;

      if (_resourceService.DecreaseCapacity(resource.Kind, resource.Amount) && _capacityEffect.FillOnAdd)
        _resourceService.TrySpend(resource.Kind, resource.Amount);
    }
  }
}