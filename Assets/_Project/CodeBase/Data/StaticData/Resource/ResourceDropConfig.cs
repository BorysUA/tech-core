using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Range = _Project.CodeBase.Data.StaticData.Common.Range;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [CreateAssetMenu(fileName = "ResourceDropConfig", menuName = "ScriptableObjects/ResourceDropConfig")]
  public class ResourceDropConfig : ScriptableObject
  {
    public ResourceDropType Type;
    public ResourceRange ResourceRange;
    public AssetReferenceGameObject PrefabReference;
  }
}