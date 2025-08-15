using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Models.Session;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Resource
{
  public class CapacityModule : BuildingModule
  {
    private readonly ISessionProgress _sessionStateModel;
    private CapacityEffect _capacityEffect;

    public CapacityModule(ISessionProgress sessionStateModel)
    {
      _sessionStateModel = sessionStateModel;
    }

    public void Setup(CapacityEffect capacityEffect) =>
      _capacityEffect = capacityEffect;

    protected override void Activate()
    {
      ResourceAmountData resource = _capacityEffect.AdditionalCapacity;

      _sessionStateModel.GetResourceModel(resource.Kind).AddCapacityBonus(resource.Amount);

      if (_capacityEffect.FillOnAdd)
        _sessionStateModel.GetResourceModel(resource.Kind).AddRuntimeAmount(resource.Amount);
    }

    protected override void Deactivate()
    {
      ResourceAmountData resource = _capacityEffect.AdditionalCapacity;

      _sessionStateModel.GetResourceModel(resource.Kind).SubtractCapacityBonus(resource.Amount);

      if (_capacityEffect.FillOnAdd)
        _sessionStateModel.GetResourceModel(resource.Kind).SubtractRuntimeAmount(resource.Amount);
    }
  }
}