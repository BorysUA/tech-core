using DG.Tweening;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services
{
  public interface ITweenFactory
  {
    Tween CreatePathTween(Transform target, Vector3[] path, float duration, Ease ease = Ease.InOutSine);
    Tween CreateFadeTween(CanvasGroup target, float endAlpha, float duration, Ease ease = Ease.InOutSine);

    Tween CreateYMovementTween(RectTransform rectTransform, float endValue, float duration,
      bool snapping = false, Ease ease = Ease.InOutSine);
  }
}