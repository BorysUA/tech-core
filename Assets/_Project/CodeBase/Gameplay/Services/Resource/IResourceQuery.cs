using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public interface IResourceQuery
  {
    ResourceStorage Get(ResourceKind kind);
  }
}