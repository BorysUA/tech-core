using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.StateMachine.States
{
  public class BootstrapState : IEnterState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly List<IBootstrapInitAsync> _onLoadInitializables;
    private readonly ILogService _logService;

    public BootstrapState(GameStateMachine gameStateMachine, List<IBootstrapInitAsync> onLoadInitializables,
      ILogService logService)
    {
      _gameStateMachine = gameStateMachine;
      _onLoadInitializables = onLoadInitializables;
      _logService = logService;
    }

    public void Enter()
    {
      EnterAsync().Forget();
    }

    private async UniTaskVoid EnterAsync()
    {
      try
      {
        await InitializeServices();
        LoadMenuScene();
      }
      catch (Exception exception)
      {
        _logService.LogError(GetType(), "Initialization failed", exception);
      }
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