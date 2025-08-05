using _Project.CodeBase.Gameplay.Constants;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IResourceReader
  {
    public ResourceKind Kind { get; }
    public ReadOnlyReactiveProperty<int> Amount { get; }
    public ReadOnlyReactiveProperty<int> Capacity { get; }
  }
}