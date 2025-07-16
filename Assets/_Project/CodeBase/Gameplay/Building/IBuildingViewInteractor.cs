using _Project.CodeBase.Gameplay.Building.Modules;

namespace _Project.CodeBase.Gameplay.Building
{
  public interface IBuildingViewInteractor
  {
    bool TryGetPublicModuleContract<TContract>(out TContract result) where TContract : class;
  }
}