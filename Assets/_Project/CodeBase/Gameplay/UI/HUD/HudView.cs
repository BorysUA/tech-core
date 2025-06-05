using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Gameplay.UI.HUD.ResourceBar;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD
{
  public class HudView : MonoBehaviour
  {
    [SerializeField] private Button _civilianBuildingsShopButton;
    [SerializeField] private Button _plotsShopButton;
    [SerializeField] private Button _extractionBuildingsShopButton;
    [SerializeField] private Button _industrialBuildingsShopButton;
    [SerializeField] private Button _settingsButton;

    [SerializeField] private ResourceBarView _resourceBarView;
    [SerializeField] private BuildingActionBar _buildingActionBar;
    [SerializeField] private NotificationView _notificationView;

    [Inject]
    public void Setup(HudViewModel hudViewModel)
    {
      _civilianBuildingsShopButton.OnClickAsObservable()
        .Subscribe(_ => hudViewModel.OpenBuildingsShopWindow(BuildingCategory.Civilian))
        .AddTo(this);

      _extractionBuildingsShopButton.OnClickAsObservable()
        .Subscribe(_ => hudViewModel.OpenBuildingsShopWindow(BuildingCategory.Extraction))
        .AddTo(this);

      _industrialBuildingsShopButton.OnClickAsObservable()
        .Subscribe(_ => hudViewModel.OpenBuildingsShopWindow(BuildingCategory.Industrial))
        .AddTo(this);

      _plotsShopButton.OnClickAsObservable()
        .Subscribe(_ => hudViewModel.OpenConstructionPlotsShopWindow())
        .AddTo(this);

      _settingsButton.OnClickAsObservable()
        .Subscribe(_ => hudViewModel.OpenSettingsWindow())
        .AddTo(this);
    }
  }
}