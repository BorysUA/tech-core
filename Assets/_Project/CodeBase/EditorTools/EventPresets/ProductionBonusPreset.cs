#if UNITY_EDITOR
using _Project.CodeBase.Data.Presets;
using _Project.CodeBase.Data.Remote;
using _Project.CodeBase.Gameplay.Constants;
using UnityEditor;

namespace _Project.CodeBase.EditorTools.EventPresets
{
  [CustomEditor(typeof(ProductionBonusEventPreset))]
  public class ProductionBonusPreset : BaseEventPresetEditor<ProductionBonusEventPreset, ProductionBoostEventData>
  {
    protected override void SetCustomFields(ProductionBoostEventData data, ProductionBonusEventPreset preset)
    {
      data.Resource = preset.Resource;
      data.Multiplier = preset.Multiplier;
    }

    protected override void OnReset(ProductionBonusEventPreset preset)
    {
      base.OnReset(preset);
      preset.Resource = ResourceKind.None;
      preset.Multiplier = 1.0f;
    }
  }
}
#endif