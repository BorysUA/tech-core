using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Common
{
  [Serializable]
  public class Range
  {
    [FormerlySerializedAs("MinAmount")] public int Min;
    [FormerlySerializedAs("MaxAmount")] public int Max;
    
    public void Validate()
    {
      if (Min > Max)
      {
        Debug.LogWarning("MinAmount cannot be greater than MaxAmount. Adjusting values.");
        Min = Max;
      }
    }
  }
}