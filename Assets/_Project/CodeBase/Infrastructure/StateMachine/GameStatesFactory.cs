using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using Zenject;

namespace _Project.CodeBase.Infrastructure.StateMachine
{
  public class GameStatesFactory
  {
    private readonly IInstantiator _instantiator;

    public GameStatesFactory(IInstantiator instantiator) =>
      _instantiator = instantiator;

    public TState CreateState<TState>() where TState : IExitState =>
      _instantiator.Instantiate<TState>();
  }
}