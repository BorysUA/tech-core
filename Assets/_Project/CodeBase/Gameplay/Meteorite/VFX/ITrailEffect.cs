using UnityEngine;

namespace _Project.CodeBase.Gameplay.Meteorite.VFX
{
  public interface ITrailEffect
  {
    void Play();
    void SetDirection(Vector3 direction);
    void Stop();
  }
}