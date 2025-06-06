using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.StateMachine.States
{
  public class BootstrapState : IState
  {
    private readonly GameStateMachine _gameStateMachine;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly IProgressService _progressService;
    private readonly IAssetProvider _assetProvider;

    public BootstrapState(GameStateMachine gameStateMachine,
      IStaticDataProvider staticDataProvider, IProgressService progressService, IAssetProvider assetProvider)
    {
      _gameStateMachine = gameStateMachine;
      _staticDataProvider = staticDataProvider;
      _progressService = progressService;
      _assetProvider = assetProvider;
    }

    public async void Enter()
    {
      DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().ConfigureAwait(true);

      if (status == DependencyStatus.Available)
      {
        Debug.Log($"[Firebase] Ready: {FirebaseApp.DefaultInstance.Name}");
      }
      else
      {
        Debug.LogError($"[Firebase] Missing libs: {status}");
      }
      await InitializeServices();
      LoadMenuScene();
    }

    private async UniTask InitializeServices()
    {
      await _assetProvider.InitializeAsync();
      await _staticDataProvider.InitializeAsync();
    }

    private void LoadMenuScene()
    {
      _gameStateMachine.Enter<LoadSceneState, string>(SceneName.MainMenu);
    }

    public void Exit()
    {
    }
  }
}