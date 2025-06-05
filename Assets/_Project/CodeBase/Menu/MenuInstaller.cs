using _Project.CodeBase.Data;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Menu.UI.Factories;
using _Project.CodeBase.Menu.UI.SaveBrowser;
using _Project.CodeBase.Menu.UI.Window;
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
      BindSignalBus();
      BindModels();
    }

    private void BindUI()
    {
      Container.Bind<WindowsCanvas>().FromInstance(_windowsCanvas).AsSingle();
      Container.Bind<WindowsRepository>().AsSingle();
      Container.BindInterfacesAndSelfTo<WindowsFactory>().AsSingle();
    }

    private void BindSignalBus()
    {
      SignalBusInstaller.Install(Container);

      Container.DeclareSignal<LoadGameplaySignal>();
    }

    private void BindViewModels()
    {
      Container.BindInterfacesAndSelfTo<MenuViewModel>().AsSingle();
      Container.BindInterfacesAndSelfTo<DifficultySelectionViewModel>().AsSingle();
      Container.BindInterfacesAndSelfTo<SaveSelectionViewModel>().AsSingle();
    }

    private void BindModels()
    {
      Container.Bind<GameplaySettings>().AsSingle();
    }

    private void BindServices()
    {
      Container.BindInterfacesAndSelfTo<GameStatesFactory>().AsSingle();
      Container.BindInterfacesAndSelfTo<WindowsService>().AsSingle();
      Container.BindInterfacesAndSelfTo<MenuUiFactory>().AsSingle();
    }

    private void BindEntryPoint()
    {
      Container.BindInterfacesAndSelfTo<MenuEntryPoint>().AsSingle().NonLazy();
    }
  }
}