using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.Presets
{
  [CreateAssetMenu(menuName = "ScriptableObjects/Presets/ProductionBonusPreset", fileName = "ProductionBonusPreset")]
  public class ProductionBonusEventPreset : EventPreset
  {
    public ResourceKind Resource = ResourceKind.None;
    public float Multiplier = 1.0f;
  }
}