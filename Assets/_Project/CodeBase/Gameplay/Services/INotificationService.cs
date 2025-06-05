using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services
{
  public interface INotificationService
  {
    void ShowMessage(string text, Color? textColor = null, Sprite icon = null, float duration = 2f,
      MessageStyle style = MessageStyle.None);
  }
}