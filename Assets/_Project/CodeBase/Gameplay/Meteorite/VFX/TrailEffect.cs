using _Project.CodeBase.Gameplay.Services.Pool;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Meteorite.VFX
{
  public class TrailEffect : MonoBehaviour, ITrailEffect, IPoolItem
  {
    [SerializeField] private ParticleSystem _flames;
    private readonly Subject<Unit> _deactivated = new();

    private Transform _vfxContainer;
    public Observable<Unit> Deactivated => _deactivated;

    public void Initialize(Transform vfxRoot)
    {
      _vfxContainer = vfxRoot;
    }

    public void Setup(Transform parent)
    {
      transform.SetParent(parent, false);
      transform.localPosition = Vector3.zero;
    }

    private void OnParticleSystemStopped()
    {
      gameObject.SetActive(false);
      _deactivated.OnNext(Unit.Default);
    }

    public void Activate() =>
      gameObject.SetActive(true);

    public void Play() =>
      _flames.Play();

    public void Stop()
    {
      transform.SetParent(_vfxContainer, true);
      _flames.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void SetDirection(Vector3 direction)
    {
      if (direction.sqrMagnitude > 0.001f)
        transform.rotation = Quaternion.LookRotation(direction);
    }
  }
}