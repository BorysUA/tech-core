using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class BuildingIndicatorView : MonoBehaviour
  {
    [SerializeField] private Image _icon;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Setup(Sprite icon) =>
      _icon.sprite = icon;

    public void SetVisible(bool shown)
    {
      _canvasGroup.alpha = shown ? 1 : 0;
      gameObject.SetActive(shown);
    }
  }
}