using _Project.CodeBase.Gameplay.Data;
using R3;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public interface IResourceBehaviour
  {
    ReadOnlyReactiveProperty<int> AvailableAmount { get; }
    public void Setup(ResourceProxy resourceProxy);
    public void Add(int amount);
    public bool CanSpend(int amount);
    public bool TrySpend(int amount);
    public bool IncreaseCapacity(int amount);
    public bool DecreaseCapacity(int amount);
  }
}