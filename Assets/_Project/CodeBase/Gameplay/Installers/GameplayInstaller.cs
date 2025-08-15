using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.InputHandlers;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Gameplay.Meteorite;
using _Project.CodeBase.Gameplay.Models.Session;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Services.Resource.ProductionModifiers;
using _Project.CodeBase.Gameplay.Services.Timers;
using _Project.CodeBase.Gameplay.Signals.Command;
using _Project.CodeBase.Gameplay.Signals.Domain;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Gameplay.UI.Effects;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.HUD;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.GameEvent;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Gameplay.UI.HUD.ResourceBar;
using _Project.CodeBase.Gameplay.UI.PopUps.ConfirmPlace;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Gameplay.UI.Windows.Settings;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Infrastructure.StateMachine;
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
      BindInfrastructureServices();
      BindUiRoot();
      BindViewModels();
      BindUiRepositories();
      BindUiFactories();
      BindCamera();
      BindSignals();
      BindInputHandlers();
      BindInputService();
      BindTrackers();
      BindGameEvents();
      BindSessionServices();
      BindSpawners();
      BindUtilityServices();
      BindUiServices();
      BindUiEffects();
      BindModels();
      BindDomainServices();
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

    private void BindGameEvents()
    {
      Container.BindInterfacesTo<LiveEventsService>().AsSingle();
      Container.Bind<EventsFactory>().AsSingle();
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
      Container.DeclareSignal<ResourcesGained>();
      Container.DeclareSignal<ResourcesSpent>();
      Container.DeclareSignal<BuildingPlaced>();
      Container.DeclareSignal<ConstructionPlotPlaced>();
      Container.DeclareSignal<BuildingDestroyed>();
    }

    private void BindCamera()
    {
      Container.Bind<CameraRigAgent>().FromInstance(_cameraRigAgent).AsSingle();
    }

    private void BindUiRoot()
    {
      Container.Bind<PopUpsCanvas>().FromInstance(_popUpsCanvas).AsSingle();
      Container.Bind<WindowsCanvas>().FromInstance(_windowsCanvas).AsSingle();
      Container.Bind<GameEventsAddressMap>().AsSingle();
    }

    private void BindUiRepositories()
    {
      Container.Bind<WindowsRepository>().AsSingle();
      Container.Bind<PopUpRepository>().AsSingle();
    }

    private void BindViewModels()
    {
      Container.Bind<PlotsShopViewModel>().AsTransient();
      Container.Bind<BuildingsShopViewModel>().AsTransient();
      Container.Bind<ConfirmPlaceViewModel>().AsTransient();
      Container.Bind<ResourceBarViewModel>().AsSingle();
      Container.Bind<HudViewModel>().AsSingle();
      Container.Bind<BuildingActionBarViewModel>().AsSingle();
      Container.Bind<SettingsViewModel>().AsSingle();
      Container.Bind<GameEventsViewModel>().AsSingle();
      Container.BindInterfacesAndSelfTo<NotificationViewModel>().AsSingle();
    }

    private void BindUiFactories()
    {
      Container.BindInterfacesTo<WindowsFactory>().AsSingle();
      Container.BindInterfacesTo<PopUpFactory>().AsSingle();
    }
    
    private void BindSessionServices()
    {
      Container.BindInterfacesAndSelfTo<GameplayPhaseFlow>().AsSingle();
      Container.BindInterfacesTo<GameSaveService>().AsSingle();
      Container.BindInterfacesTo<SessionTimer>().AsSingle();
    }

    private void BindUiServices()
    {
      Container.BindInterfacesTo<WindowsService>().AsSingle();
      Container.BindInterfacesTo<PopUpService>().AsSingle();
    }

    private void BindUiEffects()
    {
      Container.BindInterfacesTo<BuildingIndicatorsUiEffect>().AsSingle();
      Container.BindInterfacesAndSelfTo<ResourceFlyTextUiEffect>().AsSingle();
    }

    private void BindDomainServices()
    {
      Container.BindInterfacesTo<BuildingRepository>().AsSingle();
      Container.BindInterfacesTo<ResourceDropRepository>().AsSingle();

      Container.BindInterfacesTo<BuildingService>().AsSingle();
      Container.BindInterfacesTo<ConstructionPlotService>().AsSingle();
      Container.BindInterfacesTo<GridOccupancyQuery>().AsSingle();
      Container.BindInterfacesTo<ResourceService>().AsSingle();
      Container.BindInterfacesTo<ProductionModifierService>().AsSingle();
      Container.Bind<ResourceBehaviourMap>().AsSingle();

      Container.BindInterfacesTo<ResourceMutator>().AsSingle();
    }

    private void BindUtilityServices()
    {
      Container.Bind<CoordinateMapper>().AsSingle();
    }

    private void BindInfrastructureServices()
    {
      Container.BindInterfacesAndSelfTo<CommandBroker>().AsSingle();
      Container.Bind<ModuleContextResolver>().AsSingle();
      Container.Bind<ContractToModuleRegistry>().AsSingle();
    }

    private void BindSpawners()
    {
      Container.Bind<MeteoriteSpawner>().AsSingle();
    }

    private void BindModels()
    {
      Container.BindInterfacesTo<SessionStateModel>().AsSingle();
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