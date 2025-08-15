namespace _Project.CodeBase.Gameplay.Buildings
{
  public interface IBuildingViewInteractor
  {
    bool TryGetPublicModuleContract<TContract>(out TContract result) where TContract : class;
  }
}