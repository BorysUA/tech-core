using System;
using System.Threading;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.ResourceBar;
using _Project.CodeBase.Gameplay.UI.Windows.Settings;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Windows;
using _Project.CodeBase.UI.Services;
using R3;

namespace _Project.CodeBase.Gameplay.UI.HUD
{
  public class HudViewModel : IDisposable
  {
    private readonly IWindowsService _windowsService;
    private readonly ResourceBarViewModel _resourceBarViewModel;
    private readonly BuildingActionBarViewModel _buildingActionBarViewModel;
    private readonly IBuildingService _buildingService;
    private readonly CompositeDisposable _subscriptions = new();

    public HudViewModel(IWindowsService windowsService, ResourceBarViewModel resourceBarViewModel,
      BuildingActionBarViewModel buildingActionBarViewModel, IBuildingService buildingService)
    {
      _windowsService = windowsService;
      _resourceBarViewModel = resourceBarViewModel;
      _buildingActionBarViewModel = buildingActionBarViewModel;
      _buildingService = buildingService;
    }

    public void Initialize()
    {
      _buildingService.CurrentSelectedBuilding
        .Skip(1)
        .Subscribe(OnBuildingSelectionChanged)
        .AddTo(_subscriptions);
    }

    public void OpenBuildingsShopWindow(BuildingCategory category)
    {
      _windowsService.OpenWindow<BuildingsShopWindow, BuildingsShopViewModel, BuildingCategory>(category,
        token: CancellationToken.None);
    }

    public void OpenConstructionPlotsShopWindow()
    {
      _windowsService.OpenWindow<PlotsShopWindow, PlotsShopViewModel>();
    }

    public void OpenSettingsWindow() =>
      _windowsService.OpenWindow<SettingsWindow, SettingsViewModel>();

    public void Dispose() =>
      _subscriptions.Dispose();

    private void OnBuildingSelectionChanged(BuildingViewModel buildingViewModel)
    {
      if (buildingViewModel != null)
        ShowBuildingActionPanel(buildingViewModel);
      else
        HideBuildingActionPanel();
    }

    private void ShowBuildingActionPanel(BuildingViewModel building) =>
      _buildingActionBarViewModel.Show(building);

    private void HideBuildingActionPanel() =>
      _buildingActionBarViewModel.Hide();
  }
}