using _Project.CodeBase.Gameplay.Data;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public abstract class ResourceBehaviour
  {
    protected ResourceProxy ResourceData;
    public ReadOnlyReactiveProperty<int> Amount => ResourceData.Amount;

    public void Setup(ResourceProxy resourceProxy)
    {
      ResourceData = resourceProxy;
    }

    public abstract void Add(int amount);
    public abstract bool TrySpend(int amount);
  }
}