using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Gameplay.Services.CameraSystem;
using _Project.CodeBase.Gameplay.Services.Pool;
using DG.Tweening;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Meteorite.VFX
{
  public class ExplosionEffect : MonoBehaviour, IExplosionEffect, IPoolItem<PoolUnit>
  {
    [SerializeField] private ParticleSystem _vfx;

    private readonly Subject<Unit> _deactivated = new();

    private CameraRigAgent _cameraRigAgent;
    private ShakePreset _shakePreset;

    public float Progress => _vfx.totalTime > 0f ? Mathf.Clamp01(_vfx.time / _vfx.totalTime) : 0f;
    public Observable<Unit> Deactivated => _deactivated;

    [Inject]
    public void Construct(CameraRigAgent cameraRigAgent)
    {
      _cameraRigAgent = cameraRigAgent;
    }

    public void Setup(Vector3 position, float scale, ShakePreset cameraShakePreset)
    {
      transform.position = position;
      transform.localScale = Vector3.one * scale;
      _shakePreset = cameraShakePreset;
    }

    public void Activate(PoolUnit param) =>
      gameObject.SetActive(true);

    public void Play()
    {
      _vfx.Play();
      ShakeCamera();
    }

    private void OnParticleSystemStopped()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }

    private void ShakeCamera()
    {
      if (_cameraRigAgent.IsVisible(transform.position))
        _cameraRigAgent.ShakeContainer.DOShakePosition(
          _shakePreset.Duration,
          _shakePreset.Strength,
          _shakePreset.Vibrato);
    }
  }
}