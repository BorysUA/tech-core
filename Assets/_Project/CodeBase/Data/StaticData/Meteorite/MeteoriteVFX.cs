using System;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Meteorite
{
  [Serializable]
  public struct MeteoriteVFX
  {
    public MeteoriteType Type;

    [Header("Explosion")] public AssetReferenceGameObject ExplosionPrefab;
    public float ExplosionScale;

    [Header("Trail")] public AssetReferenceGameObject TrailPrefab;
    public float TrailScale;

    [Header("Camera shake")] public ShakePreset CameraShakePreset;
  }
}