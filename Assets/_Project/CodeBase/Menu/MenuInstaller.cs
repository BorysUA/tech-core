using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Menu.Services;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Menu.UI.Factories;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.Menu.UI.SaveSelection;
using _Project.CodeBase.Services.AnalyticsService.Trackers;
using _Project.CodeBase.UI.Services;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Menu
{
  public class MenuInstaller : MonoInstaller
  {
    [SerializeField] private WindowsCanvas _windowsCanvas;

    public override void InstallBindings()
    {
      BindUI();
      BindServices();
      BindEntryPoint();
      BindViewModels();
      BindSignals();
      BindTrackers();
    }

    private void BindUI()
    {
      Container.Bind<WindowsCanvas>().FromInstance(_windowsCanvas).AsSingle();
      Container.Bind<WindowsRepository>().AsSingle();
      Container.BindInterfacesAndSelfTo<WindowsFactory>().AsSingle();
    }

    private void BindSignals()
    {
      Container.DeclareSignal<GameplaySceneLoadRequested>();
    }

    private void BindViewModels()
    {
      Container.BindInterfacesAndSelfTo<MenuViewModel>().AsSingle();
      Container.BindInterfacesAndSelfTo<DifficultySelectionViewModel>().AsSingle();
      Container.BindInterfacesAndSelfTo<SaveSelectionViewModel>().AsSingle();
    }

    private void BindServices()
    {
      Container.Bind<GameStatesFactory>().AsSingle();
      Container.BindInterfacesTo<WindowsService>().AsSingle();
      Container.BindInterfacesTo<MenuUiFactory>().AsSingle();
      Container.BindInterfacesTo<GameplaySettingsBuilder>().AsSingle();
    }

    private void BindEntryPoint()
    {
      Container.BindInterfacesAndSelfTo<MenuEntryPoint>().AsSingle().NonLazy();
    }

    private void BindTrackers() =>
      Container.BindInterfacesAndSelfTo<GameplaySettingsTracker>().AsSingle();
  }
}