using System;
using System.Threading;
using _Project.CodeBase.Gameplay.Buildings;
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
    private readonly ResourceBarViewModel _resourceBar;
    private readonly BuildingActionBarViewModel _buildingActionBar;
    private readonly IBuildingService _buildingService;
    private readonly CompositeDisposable _subscriptions = new();

    public HudViewModel(IWindowsService windowsService, ResourceBarViewModel resourceBar,
      BuildingActionBarViewModel buildingActionBar, IBuildingService buildingService)
    {
      _windowsService = windowsService;
      _resourceBar = resourceBar;
      _buildingActionBar = buildingActionBar;
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

    private void OnBuildingSelectionChanged(IBuildingActionReader buildingActionReader)
    {
      if (buildingActionReader != null)
        ShowBuildingActionPanel(buildingActionReader);
      else
        HideBuildingActionPanel();
    }

    private void ShowBuildingActionPanel(IBuildingActionReader buildingActionReader) =>
      _buildingActionBar.Show(buildingActionReader);

    private void HideBuildingActionPanel() =>
      _buildingActionBar.Hide();
  }
}