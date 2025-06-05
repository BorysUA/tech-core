using _Project.CodeBase.Gameplay.UI.Factory;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.HUD.Notification
{
  public class NotificationView : MonoBehaviour
  {
    private IGameplayUiFactory _uiFactory;

    [SerializeField] private Transform _notificationsContainer;

    [Inject]
    public void Construct(NotificationViewModel viewModel, IGameplayUiFactory uiFactory)
    {
      _uiFactory = uiFactory;

      viewModel.ToShow
        .Subscribe(ShowToast)
        .AddTo(this);
    }

    private async void ShowToast(ToastData data)
    {
      NotificationMessage notificationMessage = await _uiFactory.CreateNotification(data, _notificationsContainer);
      notificationMessage.Show();
    }
  }
}