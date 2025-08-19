using _Project.CodeBase.Gameplay.Services.Pool;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class ResourceAmountItem : MonoBehaviour, IResettablePoolItem<PoolUnit>
  {
    [SerializeField] private Image _resourceIcon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private readonly Subject<Unit> _deactivated = new();
    public Observable<Unit> Deactivated => _deactivated;

    public void Setup(Sprite resourceIcon, int amount)
    {
      _resourceIcon.sprite = resourceIcon;
      _amountText.SetText("{0}", amount);
    }

    public void Activate(PoolUnit param) =>
      gameObject.SetActive(true);

    public void Deactivate()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }

    public void Reset()
    {
      _resourceIcon.sprite = null;
      _amountText.text = string.Empty;
    }
  }
}