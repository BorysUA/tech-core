using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraService;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Signals;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.HUD;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Gameplay.UI.HUD.ResourceBar;
using _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Gameplay.UI.Spawner;
using _Project.CodeBase.Gameplay.UI.Windows.Settings;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.AnalyticsService;
using _Project.CodeBase.Services.AnalyticsService.Trackers;
using _Project.CodeBase.Services.InputService;
using _Project.CodeBase.UI.Services;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Installers
{
  public class GameplayInstaller : MonoInstaller
  {
    [SerializeField] private Transform _uiRoot;
    [SerializeField] private PopUpsCanvas _popUpsCanvas;
    [SerializeField] private WindowsCanvas _windowsCanvas;
    [SerializeField] private CameraRigAgent _cameraRigAgent;

    public override void InstallBindings()
    {
      BindGameplayStateMachine();
      BindEntryPoint();
      BindFactories();
      BingServices();
      BindUi();
      BingCamera();
      BindSignals();
      BindInputHandlers();
      BindInputService();
      BindTrackers();
    }

    private void BindInputService()
    {
      Container.BindInterfacesTo<InputService>().AsSingle();
      Container.Bind<TapDetector>().AsSingle();
    }

    private void BindInputHandlers()
    {
      Container.BindInterfacesAndSelfTo<GridPlacement>().AsSingle();
      Container.BindInterfacesAndSelfTo<ResourceCollector>().AsSingle();
      Container.BindInterfacesAndSelfTo<BuildingSelector>().AsSingle();
      Container.BindInterfacesAndSelfTo<CameraMovement>().AsSingle();
    }

    private void BindGameplayStateMachine()
    {
      Container.Bind<GameplayStateMachine>().AsSingle();
    }

    private void BindTrackers()
    {
      Container.BindInterfacesAndSelfTo<ResourceAccumulationTracker>().AsSingle();
      Container.BindInterfacesAndSelfTo<BuildingPurchaseTracker>().AsSingle();
      Container.BindInterfacesAndSelfTo<BuildingLifecycleTracker>().AsSingle();
    }

    private void BindSignals()
    {
      Container.DeclareSignal<BuildingPurchaseRequested>();
      Container.DeclareSignal<ConstructionPlotPurchaseRequested>();
      Container.DeclareSignal<ResourceAmountChanged>();
      Container.DeclareSignal<ResourceDropCollected>();
      Container.DeclareSignal<BuildingPlaced>();
      Container.DeclareSignal<ConstructionPlotPlaced>();
      Container.DeclareSignal<BuildingDestroyed>();
    }

    private void BingCamera()
    {
      Container.Bind<CameraRigAgent>().FromInstance(_cameraRigAgent).AsSingle();
    }

    private void BindUi()
    {
      Container.BindInterfacesAndSelfTo<ResourceFlyTextSpawner>().AsSingle();
      Container.BindInterfacesAndSelfTo<NotificationViewModel>().AsSingle();

      Container.Bind<PopUpsCanvas>().FromInstance(_popUpsCanvas).AsSingle();
      Container.Bind<WindowsCanvas>().FromInstance(_windowsCanvas).AsSingle();

      Container.Bind<ConstructionPlotsShopViewModel>().AsTransient();
      Container.Bind<ConfirmPlaceViewModel>().AsTransient();
      Container.Bind<ResourceBarViewModel>().AsSingle();
      Container.Bind<HudViewModel>().AsSingle();
      Container.Bind<BuildingActionBarViewModel>().AsSingle();
      Container.Bind<SettingsViewModel>().AsSingle();
      Container.Bind<NotificationMessage>().AsSingle();

      Container.Bind<WindowsRepository>().AsSingle();
      Container.Bind<PopUpRepository>().AsSingle();

      Container.BindInterfacesTo<WindowsFactory>().AsSingle();
      Container.BindInterfacesTo<PopUpFactory>().AsSingle();
    }

    private void BingServices()
    {
      Container.BindInterfacesAndSelfTo<CommandBroker>().AsSingle();
      Container.Bind<CoordinateMapper>().AsSingle();
      Container.Bind<MeteoriteSpawner>().AsSingle();
      Container.Bind<ContractToModuleRegistry>().AsSingle();
      Container.BindInterfacesTo<BuildingService>().AsSingle();
      Container.BindInterfacesTo<GameSaveService>().AsSingle();
      Container.BindInterfacesTo<SessionTimer>().AsSingle();
      Container.BindInterfacesTo<StartingResourceProvider>().AsSingle();
      Container.BindInterfacesTo<WindowsService>().AsSingle();
      Container.BindInterfacesTo<GridService>().AsSingle();
      Container.BindInterfacesTo<PopUpService>().AsSingle();
      Container.BindInterfacesTo<ConstructionPlotService>().AsSingle();
      Container.BindInterfacesTo<GridOccupancyService>().AsSingle();
      Container.BindInterfacesTo<ResourceService>().AsSingle();
      Container.Bind<ResourceBehaviourMap>().AsSingle();
    }

    private void BindEntryPoint()
    {
      Container.BindInterfacesAndSelfTo<GameplayEntryPoint>().AsSingle().NonLazy();
    }

    private void BindFactories()
    {
      Container.BindInterfacesAndSelfTo<GameStatesFactory>().AsSingle();
      Container.BindInterfacesAndSelfTo<CommandFactory>().AsSingle();
      Container.BindInterfacesTo<BuildingFactory>().AsSingle();
      Container.BindInterfacesTo<VFXFactory>().AsSingle();
      Container.BindInterfacesTo<ConstructionPlotFactory>().AsCached();
      Container.BindInterfacesTo<GameplayFactory>().AsSingle();
      Container.BindInterfacesAndSelfTo<ActionFactory>().AsSingle();
      Container.BindInterfacesTo<GameplayUiFactory>().AsSingle().WithArguments(_uiRoot);
    }
  }
}