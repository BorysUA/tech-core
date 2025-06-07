using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.StateMachine.States
{
  public class BootstrapState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly List<IOnLoadInitializableAsync> _onLoadInitializables;

    public BootstrapState(GameStateMachine gameStateMachine, List<IOnLoadInitializableAsync> onLoadInitializables)
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
      foreach (IOnLoadInitializableAsync service in _onLoadInitializables)
        await service.InitializeAsync();
    }

    private void LoadMenuScene()
    {
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.MainMenu);
    }
  }
}