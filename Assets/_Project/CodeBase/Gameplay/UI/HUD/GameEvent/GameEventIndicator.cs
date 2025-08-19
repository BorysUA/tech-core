using System;
using R3;
using TMPro;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.HUD.GameEvent
{
  public class GameEventIndicator : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private TextMeshProUGUI _statusText;

    public void Initialize(bool isActive, ReadOnlyReactiveProperty<TimeSpan> gameEventCountdown)
    {
      gameEventCountdown
        .Subscribe(timeNow =>
        {
          _countdownText.SetText("{0:00}:{1:00}:{2:00}", timeNow.Hours, timeNow.Minutes, timeNow.Seconds);
        })
        .AddTo(this);

      _statusText.SetText(isActive ? "NOW" : "SOON");
    }

    public void Destroy()
    {
      Destroy(gameObject);
    }
  }
}