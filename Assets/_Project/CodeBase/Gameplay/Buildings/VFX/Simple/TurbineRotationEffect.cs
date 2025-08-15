using DG.Tweening;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.VFX.Simple
{
  public class TurbineRotationEffect : SimpleEffect
  {
    [SerializeField] private Transform _target;
    [SerializeField] private float _rotationSpeed = 45f;
    [SerializeField] private UnityEngine.Vector3 _axis = UnityEngine.Vector3.up;

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

    protected override void OnPlay() =>
      _tween.Play();

    protected override void OnPause() =>
      _tween.Pause();

    protected override void OnStop() =>
      _tween.Pause();

    private void OnDestroy() =>
      _tween?.Kill();
  }
}