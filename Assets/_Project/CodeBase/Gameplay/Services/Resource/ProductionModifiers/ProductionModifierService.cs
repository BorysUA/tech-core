using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers
{
  public class ProductionModifierService : IProductionModifierService
  {
    private const float DefaultModifier = 1f;
    private readonly Dictionary<ResourceKind, List<ProductionMultiplier>> _modifiers = new();

    public float GetMultiplier(ResourceKind kind)
    {
      if (!_modifiers.TryGetValue(kind, out List<ProductionMultiplier> modifiers))
        return 1f;

      return modifiers.Aggregate(DefaultModifier, (current, modifier) => current * modifier.Multiplier);
    }

    public void AddModifier(ProductionMultiplier modifier)
    {
      if (!_modifiers.TryGetValue(modifier.Kind, out List<ProductionMultiplier> modifiers))
      {
        modifiers = new List<ProductionMultiplier>();
        _modifiers.Add(modifier.Kind, modifiers);
      }

      modifiers.Add(modifier);
    }

    public void RemoveModifier(ProductionMultiplier modifier)
    {
      if (_modifiers.TryGetValue(modifier.Kind, out List<ProductionMultiplier> list))
      {
        list.Remove(modifier);

        if (list.Count == 0)
          _modifiers.Remove(modifier.Kind);
      }
    }
  }
}