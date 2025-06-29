using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers
{
  public interface IProductionModifierService
  {
    float GetMultiplier(ResourceKind kind);
    void AddModifier(ProductionMultiplier modifier);
    void RemoveModifier(ProductionMultiplier modifier);
  }
}