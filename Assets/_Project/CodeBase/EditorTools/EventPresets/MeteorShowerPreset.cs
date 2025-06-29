#if UNITY_EDITOR
using _Project.CodeBase.Data.Presets;
using _Project.CodeBase.Data.Remote;
using UnityEditor;

namespace _Project.CodeBase.EditorTools.EventPresets
{
  [CustomEditor(typeof(MeteorShowerEventPreset))]
  public class MeteorShowerPreset : BaseEventPresetEditor<MeteorShowerEventPreset, MeteorShowerEventData>
  {
    protected override void SetCustomFields(MeteorShowerEventData data, MeteorShowerEventPreset preset)
    {
      data.Multiplier = preset.Multiplier;
    }

    protected override void OnReset(MeteorShowerEventPreset preset)
    {
      base.OnReset(preset);
      preset.Multiplier = 1f;
    }
  }
}
#endif