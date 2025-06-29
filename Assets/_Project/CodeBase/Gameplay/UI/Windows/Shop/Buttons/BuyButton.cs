using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.UI.Windows.Common;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Buttons
{
  public class BuyButton : ObservableButton, IPoolItem<Transform>
  {
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _priceIcon;

    private readonly Subject<Unit> _deactivated = new();

    public Observable<Unit> Deactivated => _deactivated;

    public void Setup(string title, Sprite itemIcon, int cost, Sprite priceIcon)
    {
      _title.text = title;
      _price.text = cost.ToString();
      _itemIcon.sprite = itemIcon;
      _priceIcon.sprite = priceIcon;
    }

    public void Activate(Transform parent)
    {
      gameObject.SetActive(true);
      transform.SetParent(parent, false);
    }

    public void Deactivate()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }
  }
}