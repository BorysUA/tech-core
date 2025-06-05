using Zenject;

namespace _Project.CodeBase.Gameplay.Building.Actions.Common
{
  public class ActionFactory
  {
    private IInstantiator _instantiator;

    public ActionFactory(IInstantiator instantiator) =>
      _instantiator = instantiator;

    public TAction CreateAction<TAction>() where TAction : IBuildingAction =>
      _instantiator.Instantiate<TAction>();
  }
}