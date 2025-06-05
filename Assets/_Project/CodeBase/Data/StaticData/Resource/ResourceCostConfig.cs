using System;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [Serializable]
  public class ResourceCostConfig
  {
    public ResourceConfig Resource;
    public int Amount;
  }
}