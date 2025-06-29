using UnityEngine;

namespace _Project.CodeBase.Data.Presets
{
  [CreateAssetMenu(menuName = "ScriptableObjects/Presets/MeteorShowerPreset", fileName = "MeteorShowerPreset")]
  public class MeteorShowerEventPreset : EventPreset
  {
    public float Multiplier = 1.0f;
  }
}