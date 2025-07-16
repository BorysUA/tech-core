using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.VFX.Simple
{
  [RequireComponent(typeof(ParticleSystem))]
  public class SmokeEffect : SimpleEffect
  {
    private ParticleSystem _particleSystem;

    private void Awake() =>
      _particleSystem = GetComponent<ParticleSystem>();

    public override void Play()
    {
      _particleSystem.Play();
    }

    protected override void Pause()
    {
      _particleSystem.Pause();
    }

    public override void Stop()
    {
      _particleSystem.Stop();
    }
  }
}