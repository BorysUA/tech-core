using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [CreateAssetMenu(fileName = "ResourceSpotsConfig", menuName = "ScriptableObjects/ResourceSpotsConfig")]
  public class ResourceSpotMap : ScriptableObject
  {
    public ResourceSpotEntry[] Spots;
  }
}