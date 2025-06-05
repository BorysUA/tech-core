using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using R3;

namespace _Project.CodeBase.Gameplay.Data
{
  public class ResourceProxy
  {
    public ResourceKind Kind { get; }
    public ReactiveProperty<int> Amount { get; }
    public ReactiveProperty<int> Capacity { get; }

    public ResourceProxy(GameResourceData gameResourceData)
    {
      Kind = gameResourceData.Kind;
      Amount = new ReactiveProperty<int>(gameResourceData.Amount);
      Capacity = new ReactiveProperty<int>(gameResourceData.Capacity);

      Amount.Subscribe(value => { gameResourceData.Amount = value; });
      Capacity.Subscribe(value => { gameResourceData.Capacity = value; });
    }
  }
}