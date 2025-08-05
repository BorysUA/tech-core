using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;

namespace _Project.CodeBase.Infrastructure.StateMachine.States
{
  public class LoadSceneState : IPayloadState<string>
  {
    private readonly ISceneLoader _sceneLoader;
    private readonly GameStateMachine _gameStateMachine;

    public LoadSceneState(GameStateMachine gameStateMachine, ISceneLoader sceneLoader)
    {
      _sceneLoader = sceneLoader;
      _gameStateMachine = gameStateMachine;
    }

    public void Enter(string sceneName)
    {
      _gameStateMachine.ClearSceneStates();
      _sceneLoader.LoadScene(sceneName);
    }
  }
}