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
    private readonly List<IBootstrapInitAsync> _onLoadInitAsync;
    private readonly List<IBootstrapInit> _onLoadInit;
    private readonly ILogService _logService;

    public BootstrapState(GameStateMachine gameStateMachine, List<IBootstrapInitAsync> onLoadInitAsync,
      ILogService logService, List<IBootstrapInit> onLoadInit)
    {
      _gameStateMachine = gameStateMachine;
      _onLoadInitAsync = onLoadInitAsync;
      _logService = logService;
      _onLoadInit = onLoadInit;
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

      foreach (IBootstrapInit service in _onLoadInit)
        service.Initialize();

      foreach (IBootstrapInitAsync service in _onLoadInitAsync)
        loadingTasks.Add(service.InitializeAsync());

      await UniTask.WhenAll(loadingTasks);
    }

    private void LoadMenuScene()
    {
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.MainMenu);
    }
  }
}