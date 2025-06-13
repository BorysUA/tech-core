using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Menu.States;
using Zenject;

namespace _Project.CodeBase.Menu
{
  public class MenuEntryPoint : IInitializable
  {
    private GameStatesFactory _gameStatesFactory;
    private GameStateMachine _stateMachine;

    public MenuEntryPoint(GameStatesFactory gameStatesFactory, GameStateMachine stateMachine)
    {
      _gameStatesFactory = gameStatesFactory;
      _stateMachine = stateMachine;
    }

    public void Initialize()
    {
      _stateMachine.RegisterSceneState(_gameStatesFactory.CreateState<LoadMainMenuState>());
      _stateMachine.RegisterSceneState(_gameStatesFactory.CreateState<MainMenuState>());
      
      _stateMachine.Enter<LoadMainMenuState>();
    }
  }
}