using DG.Tweening;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.VFX.Simple
{
  public class TurbineRotationEffect : SimpleEffect
  {
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 45f;
    [SerializeField] private Vector3 _axis = Vector3.up;

    private Tween _tween;

    private void Awake()
    {
      _tween = _target
        .DOLocalRotate(_axis * 360f, 360f / _rotationSpeed, RotateMode.LocalAxisAdd)
        .SetEase(Ease.Linear)
        .SetLoops(-1)
        .Pause()
        .SetAutoKill(false);
    }

    public override void Play() =>
      _tween.Play();

    protected override void Pause() =>
      _tween.Pause();

    public override void Stop() =>
      _tween.Pause();

    private void OnDestroy() =>
      _tween?.Kill();
  }
}