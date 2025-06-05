using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [CreateAssetMenu(fileName = "ResourceData", menuName = "ScriptableObjects/ResourceData", order = 0)]
  public class ResourceConfig : ScriptableObject
  {
    public ResourceKind Kind;
    public ResourceUsage Usage;
    public int Price;
    public int Capacity;
    public Sprite Icon;
    public string ResourceName;
  }
}