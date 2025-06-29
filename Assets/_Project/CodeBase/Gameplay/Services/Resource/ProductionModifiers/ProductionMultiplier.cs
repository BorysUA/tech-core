using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers
{
  public class ProductionMultiplier
  {
    public ResourceKind Kind { get; }
    public float Multiplier { get; }

    public ProductionMultiplier(ResourceKind kind, float value)
    {
      Kind = kind;
      Multiplier = value;
    }
  }
}