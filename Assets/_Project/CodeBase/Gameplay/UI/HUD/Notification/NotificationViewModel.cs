using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.HUD.Notification
{
  public class NotificationViewModel : INotificationService
  {
    private const float MessageDelay = 0.3f;

    private readonly Queue<ToastData> _queue = new();
    private readonly Subject<ToastData> _toShow = new();
    private bool _isShowing;
    public Observable<ToastData> ToShow => _toShow;

    public void ShowMessage(string text, Color? textColor = null, Sprite icon = null, float duration = 2f,
      MessageStyle style = MessageStyle.None)
    {
      ToastData toastData = new ToastData(text, textColor, icon, duration, style);
      _queue.Enqueue(toastData);
      
      if (!_isShowing)
        ProcessQueueAsync().Forget();
    }

    private async UniTaskVoid ProcessQueueAsync()
    {
      while (_queue.Count > 0)
      {
        _isShowing = true;
        _toShow.OnNext(_queue.Dequeue());
        await UniTask.Delay(TimeSpan.FromSeconds(MessageDelay));
      }

      _isShowing = false;
    }
  }
}