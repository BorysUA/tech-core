using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.StateMachine.States
{
  public class BootstrapState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly List<IBootstrapInitAsync> _onLoadInitializables;

    public BootstrapState(GameStateMachine gameStateMachine, List<IBootstrapInitAsync> onLoadInitializables)
    {
      _gameStateMachine = gameStateMachine;
      _onLoadInitializables = onLoadInitializables;
    }

    public async void Enter()
    {
      await InitializeServices();
      LoadMenuScene();
    }

    public void Exit()
    {
    }

    private async UniTask InitializeServices()
    {
      List<UniTask> loadingTasks = new List<UniTask>();

      foreach (IBootstrapInitAsync service in _onLoadInitializables)
        loadingTasks.Add(service.InitializeAsync());

      await UniTask.WhenAll(loadingTasks);
    }

    private void LoadMenuScene()
    {
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.MainMenu);
    }
  }
}