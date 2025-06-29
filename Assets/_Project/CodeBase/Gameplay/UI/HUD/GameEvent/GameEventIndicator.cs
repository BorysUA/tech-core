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
        .Subscribe(timeNow => _countdownText.text = timeNow.ToString(@"hh\:mm\:ss"))
        .AddTo(this);

      _statusText.text = isActive ? "NOW" : "SOON";
    }

    public void Destroy()
    {
      Destroy(gameObject);
    }
  }
}