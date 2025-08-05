using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Models.Session;
using R3;

namespace _Project.CodeBase.Gameplay.Resource.Behaviours
{
  public interface IResourceBehaviour
  {
    ReadOnlyReactiveProperty<int> TotalAmount { get; }
    ReadOnlyReactiveProperty<int> TotalCapacity { get; }
    ResourceKind Kind { get; }
    public void Setup(IResourceReader resourceProxy, ResourceSessionModel resourceSessionModel);
  }
}