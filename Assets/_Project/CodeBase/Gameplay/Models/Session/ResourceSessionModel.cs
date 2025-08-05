using _Project.CodeBase.Utility;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Session
{
  public class ResourceSessionModel
  {
    private readonly ReactiveProperty<int> _runtimeCapacityBonus = new(0);
    private readonly ReactiveProperty<int> _runtimeAmount = new(0);

    public ReadOnlyReactiveProperty<int> RuntimeCapacityBonus => _runtimeCapacityBonus;
    public ReadOnlyReactiveProperty<int> RuntimeAmount => _runtimeAmount;

    public void AddCapacityBonus(int delta) => _runtimeCapacityBonus.Value += delta;
    public void AddRuntimeAmount(int delta) => _runtimeAmount.Value += delta;
    public void SubtractCapacityBonus(int delta) => _runtimeCapacityBonus.Value -= delta;
    public void SubtractRuntimeAmount(int delta) => _runtimeAmount.Value -= delta;
  }
}