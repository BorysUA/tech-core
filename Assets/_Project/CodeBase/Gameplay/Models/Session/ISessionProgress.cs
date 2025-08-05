using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Models.Session
{
  public interface ISessionProgress
  {
    ResourceSessionModel GetResourceModel(ResourceKind kind);
  }
}