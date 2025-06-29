using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Meteorite.VFX;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.Factories
{
  public interface IVFXFactory
  {
    public UniTask<IExplosionEffect> CreateExplosionEffect(MeteoriteType meteoriteType, Vector3 position);
    public UniTask<ITrailEffect> CreateTrailEffect(MeteoriteType meteoriteType, Transform parent);
  }
}