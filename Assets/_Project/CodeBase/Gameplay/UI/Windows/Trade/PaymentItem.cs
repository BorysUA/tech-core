using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class PaymentItem : MonoBehaviour
  {
    [SerializeField] private Image _resourceIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    public void Setup(Sprite resourceIcon, int amount)
    {
      _resourceIcon.sprite = resourceIcon;
      _amountText.text = amount.ToString();
    }

    public void Activate() =>
      gameObject.SetActive(true);

    public void Deactivate() =>
      gameObject.SetActive(false);
  }
}