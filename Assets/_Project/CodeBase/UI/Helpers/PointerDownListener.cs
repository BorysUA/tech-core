using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.CodeBase.UI.Helpers
{
  public class PointerDownListener : MonoBehaviour, IPointerDownHandler
  {
    private readonly Subject<Unit> _pointerDown = new();
    public Observable<Unit> PointerDown => _pointerDown;

    public void OnPointerDown(PointerEventData eventData)
    {
      _pointerDown.OnNext(Unit.Default);
    }
  }
}