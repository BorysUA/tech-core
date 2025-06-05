using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [Serializable]
  public class ResourceFlowConfig
  {
    public ResourceConfig ResourceConfig;
    public int AmountPerTick;

    [Header("Interval in seconds between ticks")]
    public float TickInterval;
  }
}