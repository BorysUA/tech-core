using System.Threading;
using _Project.CodeBase.Gameplay.Building;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.ResourceBar;
using _Project.CodeBase.Gameplay.UI.Windows.Settings;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Windows;
using _Project.CodeBase.UI.Services;

namespace _Project.CodeBase.Gameplay.UI.HUD
{
  public class HudViewModel
  {
    private readonly IWindowsService _windowsService;
    private readonly ResourceBarViewModel _resourceBarViewModel;
    private readonly BuildingActionBarViewModel _buildingActionBarViewModel;

    public HudViewModel(IWindowsService windowsService, ResourceBarViewModel resourceBarViewModel,
      BuildingActionBarViewModel buildingActionBarViewModel)
    {
      _windowsService = windowsService;
      _resourceBarViewModel = resourceBarViewModel;
      _buildingActionBarViewModel = buildingActionBarViewModel;
    }

    public void OpenBuildingsShopWindow(BuildingCategory category)
    {
      _windowsService.OpenWindow<ShopWindow, BuildingsShopViewModel, BuildingCategory>(category, false,
        CancellationToken.None);
    }

    public void OpenConstructionPlotsShopWindow()
    {
      _windowsService.OpenWindow<ShopWindow, ConstructionPlotsShopViewModel>();
    }

    public void ShowBuildingActionPanel(BuildingViewModel building) =>
      _buildingActionBarViewModel.Show(building);

    public void HideBuildingActionPanel() =>
      _buildingActionBarViewModel.Hide();

    public void OpenSettingsWindow()
    {
      _windowsService.OpenWindow<SettingsWindow, SettingsViewModel>();
    }
  }
}