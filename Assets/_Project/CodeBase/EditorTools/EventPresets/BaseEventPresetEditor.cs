#if UNITY_EDITOR
using System;
using System.Globalization;
using _Project.CodeBase.Data.Presets;
using _Project.CodeBase.Data.Remote;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace _Project.CodeBase.EditorTools.EventPresets
{
  public abstract class BaseEventPresetEditor<TPreset, TData> : Editor
    where TPreset : EventPreset
    where TData : BaseEventData, new()
  {
    private readonly string[] _formats =
    {
      "O",
      "MM/dd/yyyy HH:mm:ss",
      "dd/MM/yyyy HH:mm:ss",
      "dd.MM.yyyy HH:mm:ss",
    };

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();

      TPreset preset = (TPreset)target;

      GUILayout.Space(10);
      if (GUILayout.Button("Copy validated JSON to clipboard"))
      {
        TData data = BuildData(preset);
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        EditorGUIUtility.systemCopyBuffer = json;
        Debug.Log($"{typeof(TData).Name} JSON copied to clipboard:\n" + json);
      }

      GUILayout.Space(10);
      if (GUILayout.Button("Reset"))
      {
        preset.Enabled = true;
        preset.StartUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        preset.EndUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        OnReset(preset);
      }
    }

    protected virtual void OnReset(TPreset preset)
    {
      preset.Enabled = true;
      preset.StartUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
      preset.EndUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    }

    protected abstract void SetCustomFields(TData data, TPreset preset);

    private TData BuildData(TPreset preset)
    {
      TData data = new TData();
      SetCommonFields(data, preset);
      SetCustomFields(data, preset);
      return data;
    }

    private void SetCommonFields(TData data, EventPreset preset)
    {
      Type dataType = data.GetType();
      dataType.GetField("Enabled").SetValue(data, preset.Enabled);
      dataType.GetField("StartUtc").SetValue(data, ParseUtc(preset.StartUtc));
      dataType.GetField("EndUtc").SetValue(data, ParseUtc(preset.EndUtc));
    }

    private string ParseUtc(string input)
    {
      return DateTime.ParseExact(input, _formats, CultureInfo.InvariantCulture,
        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal).ToString("O");
    }
  }
}
#endif