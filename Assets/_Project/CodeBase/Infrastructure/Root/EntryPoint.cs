using System;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.StateMachine.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.CodeBase.Infrastructure.Root
{
  public class EntryPoint : IInitializable
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly GameStatesFactory _gameStatesFactory;
    private readonly ISceneLoader _sceneLoader;
    
    private event Action SceneLoaded;

    public EntryPoint(GameStateMachine gameStateMachine, ISceneLoader sceneLoader, GameStatesFactory gameStatesFactory)
    {
      _gameStateMachine = gameStateMachine;
      _sceneLoader = sceneLoader;
      _gameStatesFactory = gameStatesFactory;
    }

    public void Initialize()
    {
      SetupGame();
      RunGame();
    }

    private void SetupGame()
    {
      Application.targetFrameRate = 60;
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void RunGame()
    {
      RegisterGlobalStates();

#if UNITY_EDITOR
      Scene activeScene = SceneManager.GetActiveScene();

      if (activeScene.buildIndex != (int)SceneIndex.Boot)
      {
        LoadBootScene();
        return;
      }
#endif

      EnterBootstrapState();
    }

    private void RegisterGlobalStates()
    {
      _gameStateMachine.RegisterState(_gameStatesFactory.CreateState<BootstrapState>());
      _gameStateMachine.RegisterState(_gameStatesFactory.CreateState<LoadSceneState>());
    }

    private void LoadBootScene()
    {
      SceneLoaded += EnterBootstrapState;
      _sceneLoader.LoadScene(SceneName.Boot, SceneLoaded);
    }

    private void EnterBootstrapState()
    {
      _gameStateMachine.Enter<BootstrapState>();
    }
  }
}