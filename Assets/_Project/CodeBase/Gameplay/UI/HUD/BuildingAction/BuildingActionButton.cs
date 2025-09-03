using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.UI.Windows.Common;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.HUD.BuildingAction
{
  public class BuildingActionButton : ObservableButton, IPoolItem<PoolUnit>
  {
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _icon;

    private readonly Subject<Unit> _deactivated = new();
    public Observable<Unit> Deactivated => _deactivated;

    public void Initialize(int index) =>
      transform.SetSiblingIndex(index);

    public void Setup(string title, Sprite icon)
    {
      _title.text = title;
      _icon.sprite = icon;
    }

    public void Activate(PoolUnit param) =>
      gameObject.SetActive(true);

    public void Deactivate()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }
  }
}