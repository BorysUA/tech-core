using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IResourceWriter : IResourceReader
  {
    public new ReactiveProperty<int> Amount { get; }
    public new ReactiveProperty<int> Capacity { get; }
  }
}