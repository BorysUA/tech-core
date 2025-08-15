using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.VFX.Simple
{
  [RequireComponent(typeof(ParticleSystem))]
  public class SmokeEffect : SimpleEffect
  {
    private ParticleSystem _particleSystem;

    private void Awake() =>
      _particleSystem = GetComponent<ParticleSystem>();

    protected override void OnPlay()
    {
      _particleSystem.Play();
    }

    protected override void OnPause()
    {
      _particleSystem.Pause();
    }

    protected override void OnStop()
    {
      _particleSystem.Stop();
    }
  }
}