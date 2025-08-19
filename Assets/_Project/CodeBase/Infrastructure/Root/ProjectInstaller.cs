using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.AssetsPipeline;
using _Project.CodeBase.Infrastructure.Services.ProgressProvider;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Infrastructure.Signals;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Infrastructure.UI;
using _Project.CodeBase.Services;
using _Project.CodeBase.Services.AnalyticsService;
using _Project.CodeBase.Services.AnalyticsService.Trackers;
using _Project.CodeBase.Services.DateTimeService;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.Services.RemoteConfigsService;
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
      BindInfrastructureServices();
      BindUI();
      BindAnalytics();
      BindRemoteConfigs();
      BindSignals();
      BindTrackers();
      BindCoreMonoComponents();
      BindProgressServices();
      BindUtilityServices();
    }

    private void BindSignals()
    {
      SignalBusInstaller.Install(Container);

      Container.DeclareSignal<AppLifecycleChanged>();
    }

    private void BindTrackers() =>
      Container.BindInterfacesAndSelfTo<SessionMetadataTracker>().AsSingle();

    private void BindUI()
    {
      Container.Bind<LoadScreen>().FromInstance(LoadScreen).AsSingle();
      Container.Bind<AddressMap>().AsSingle();
    }

    private void BindAnalytics()
    {
      Container.BindInterfacesTo<AnalyticsServiceProxy>().AsSingle();
      Container.Bind<FirebaseAnalyticsService>().AsSingle();
      Container.Bind<NoneAnalyticsService>().AsSingle();
      Container.BindInterfacesTo<FirebaseBootstrap>().AsSingle();
    }

    private void BindRemoteConfigs()
    {
      Container.Bind<RemoteConfigPatcher>().AsSingle();
      Container.BindInterfacesTo<RemoteConfigsProxy>().AsSingle();
      Container.Bind<FirebaseRemoteConfigService>().AsSingle();
      Container.Bind<NoneRemoteConfigService>().AsSingle();
    }


    private void BindProgressServices()
    {
      Container.BindInterfacesAndSelfTo<PersistentProgressProvider>().AsSingle();
      Container.BindInterfacesTo<JsonSaveStorageService>().AsSingle();
      Container.BindInterfacesTo<DateTimeService>().AsSingle();
    }

    private void BindUtilityServices()
    {
      Container.Bind<InputSystemActions>().AsSingle();
      Container.Bind<ObjectFactory>().AsSingle();
      Container.BindInterfacesTo<TimerFactory>().AsSingle();
      Container.BindInterfacesTo<TweenFactory>().AsSingle();
    }

    private void BindInfrastructureServices()
    {
      Container.BindInterfacesTo<AtlasResolver>().AsSingle();
      Container.BindInterfacesTo<SceneLoader>().AsSingle();
      Container.BindInterfacesTo<AssetProvider>().AsSingle();
      Container.BindInterfacesTo<StaticDataProvider>().AsSingle();
      Container.BindInterfacesTo<DataTransferService>().AsSingle();
      Container.BindInterfacesTo<LogService>().AsSingle();
    }

    private void BindCoreMonoComponents()
    {
      Container.BindInterfacesAndSelfTo<CoroutineRunner>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName("COROUTINE RUNNER")
        .UnderTransform(transform)
        .AsSingle();

      Container.Bind<UnityAppLifecycleBroadcaster>()
        .FromNewComponentOnNewGameObject()
        .WithGameObjectName("GAME LIFECYCLE")
        .UnderTransform(transform)
        .AsSingle()
        .NonLazy();
    }

    private void BindEntryPoint() =>
      Container.BindInterfacesAndSelfTo<EntryPoint>().AsSingle().NonLazy();

    private void BindGameStateMachine() =>
      Container.Bind<GameStateMachine>().AsSingle();

    private void BindGameFactory() =>
      Container.Bind<GameStatesFactory>().AsSingle();
  }
}