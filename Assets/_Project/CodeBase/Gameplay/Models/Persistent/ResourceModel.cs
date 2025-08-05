using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent
{
  public class ResourceModel : IResourceWriter
  {
    public ResourceKind Kind { get; }
    public ReactiveProperty<int> Amount { get; }
    public ReactiveProperty<int> Capacity { get; }

    ReadOnlyReactiveProperty<int> IResourceReader.Capacity => Capacity;
    ReadOnlyReactiveProperty<int> IResourceReader.Amount => Amount;

    public ResourceModel(GameResourceData gameResourceData)
    {
      Kind = gameResourceData.Kind;
      Capacity = new ReactiveProperty<int>(gameResourceData.Capacity);
      Amount = new ReactiveProperty<int>(gameResourceData.Amount);

      Amount.Subscribe(value => { gameResourceData.Amount = value; });
      Capacity.Subscribe(value => { gameResourceData.Capacity = value; });
    }
  }
}