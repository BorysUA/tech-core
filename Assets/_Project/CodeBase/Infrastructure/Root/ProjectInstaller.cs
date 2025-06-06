using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.UI;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Services.TimeCounter;
using Zenject;

namespace _Project.CodeBase.Infrastructure.Root
{
  public class ProjectInstaller : MonoInstaller
  {
    public LoadScreen LoadScreen;

    public override void InstallBindings()
    {
      BindEntryPoint();
      BindGameStateMachine();
      BindGameFactory();
      BindServices();
      BindUI();
    }

    private void BindUI() =>
      Container.Bind<LoadScreen>().FromInstance(LoadScreen).AsSingle();

    private void BindServices()
    {
      Container.BindInterfacesTo<CoroutineRunner>().FromNewComponentOnNewGameObject()
        .WithGameObjectName("COROUTINE RUNNER").AsSingle();

      Container.BindInterfacesTo<SceneLoader>().AsSingle();
      Container.BindInterfacesTo<TweenFactory>().AsSingle();
      Container.BindInterfacesTo<AssetProvider>().AsSingle();
      Container.BindInterfacesTo<StaticDataProvider>().AsSingle();
      Container.BindInterfacesTo<DataTransferService>().AsSingle();
      Container.BindInterfacesTo<TimerFactory>().AsSingle();
      Container.BindInterfacesTo<LogService>().AsSingle();
      Container.BindInterfacesTo<ProgressService>().AsSingle();
      Container.BindInterfacesTo<JsonSaveStorageService>().AsSingle();
      Container.Bind<InputSystemActions>().AsSingle();
      Container.Bind<AddressMap>().AsSingle();
    }

    private void BindEntryPoint()
    {
      Container.BindInterfacesAndSelfTo<EntryPoint>().AsSingle().NonLazy();
    }

    private void BindGameStateMachine() =>
      Container.Bind<GameStateMachine>().AsSingle();

    private void BindGameFactory() =>
      Container.Bind<GameStatesFactory>().AsSingle();
  }
}