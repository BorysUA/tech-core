using R3;
using TMPro;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class WaitingView : MonoBehaviour, ITradeContentView
  {
    [SerializeField] private TextMeshProUGUI _timerText;
    private DisposableBag _disposable;

    public void Show(ReadOnlyReactiveProperty<float> nextOfferCountdown)
    {
      nextOfferCountdown
        .Subscribe(UpdateTimer)
        .AddTo(ref _disposable);

      gameObject.SetActive(true);
    }

    public void Clear()
    {
      _disposable.Clear();
      gameObject.SetActive(false);
    }

    private void UpdateTimer(float time) =>
      _timerText.text = FormatTime(time);

    private string FormatTime(float totalSeconds)
    {
      int minutes = (int)(totalSeconds / 60);
      int seconds = (int)(totalSeconds % 60);
      return $"{minutes:D2}:{seconds:D2}";
    }
  }
}