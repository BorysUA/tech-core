using DG.Tweening;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Factories
{
  public class TweenFactory : ITweenFactory
  {
    public Tween CreatePathTween(Transform target, Vector3[] path, float duration, Ease ease = Ease.InOutSine) =>
      target.DOPath(path, duration, PathType.CatmullRom)
        .SetOptions(false)
        .SetEase(ease);

    public Tween CreateFadeTween(CanvasGroup target, float endAlpha, float duration, Ease ease = Ease.InOutSine) =>
      target.DOFade(endAlpha, duration)
        .SetEase(ease)
        .SetAutoKill(false);

    public Tween CreateYMovementTween(RectTransform rectTransform, float endValue, float duration,
      bool snapping = false, Ease ease = Ease.InOutSine) =>
      rectTransform.DOAnchorPosY(endValue, duration, snapping)
        .SetEase(ease)
        .SetAutoKill(false);
  }
}