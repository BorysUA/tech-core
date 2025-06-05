using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.CodeBase.Data.StaticData.Meteorite
{
  [CreateAssetMenu(fileName = "Meteorite", menuName = "ScriptableObjects/Meteorite", order = 0)]
  public class MeteoriteConfig : ScriptableObject
  {
    public MeteoriteType Type;
    public AssetReferenceGameObject PrefabReference;
    public List<ResourceDropConfig> ResourceDrops;
    public int Damage;
    public Vector2Int ExplosionArea;
    public float MoveSpeed;
    public float RotationSpeed;

    public void OnValidate()
    {
      foreach (ResourceDropConfig resourceDrop in ResourceDrops) 
        resourceDrop.ResourceRange.Amount.Validate();
    }
  }
}