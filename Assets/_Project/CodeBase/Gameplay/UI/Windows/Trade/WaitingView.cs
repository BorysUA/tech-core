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
      _timerText.SetText("{0:00}:{1:00}", (int)(time / 60), (int)(time % 60));
  }
}