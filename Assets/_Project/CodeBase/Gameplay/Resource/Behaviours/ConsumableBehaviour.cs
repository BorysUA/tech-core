using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Models.Session;
using R3;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public class ConsumableBehaviour : IResourceBehaviour
  {
    private IResourceReader _resourceReader;

    public ResourceKind Kind { get; }
    public ReadOnlyReactiveProperty<int> TotalAmount { get; private set; }
    public ReadOnlyReactiveProperty<int> TotalCapacity { get; private set; }

    public ConsumableBehaviour(ResourceKind kind)
    {
      Kind = kind;
    }

    public void Setup(IResourceReader resourceProxy, ResourceSessionModel resourceSessionModel)
    {
      _resourceReader = resourceProxy;

      TotalAmount = _resourceReader.Amount
        .ToStabilizedReadOnlyReactiveProperty();

      TotalCapacity = _resourceReader.Capacity
        .CombineLatest(resourceSessionModel.RuntimeCapacityBonus, (baseCap, bonus) => baseCap + bonus)
        .ToStabilizedReadOnlyReactiveProperty();
    }
  }
}