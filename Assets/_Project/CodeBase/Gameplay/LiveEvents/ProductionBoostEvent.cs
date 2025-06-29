using _Project.CodeBase.Data.Remote;
using _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public class ProductionBoostEvent : GameEventBase
  {
    private readonly IProductionModifierService _productionModifierService;
    private ProductionBoostEventData _eventData;
    private ProductionMultiplier _productionMultiplier;

    public override GameEventType Type => GameEventType.ProductionBoostEvent;

    public ProductionBoostEvent(IProductionModifierService productionModifierService)
    {
      _productionModifierService = productionModifierService;
    }

    public override void Initialize(BaseEventData eventData)
    {
      base.Initialize(eventData);
      _eventData = eventData as ProductionBoostEventData;
    }

    protected override void OnActivate()
    {
      _productionMultiplier = new ProductionMultiplier(_eventData.Resource, _eventData.Multiplier);
      _productionModifierService.AddModifier(_productionMultiplier);
    }

    protected override void OnDeactivate()
    {
      _productionModifierService.RemoveModifier(_productionMultiplier);
    }
  }
}