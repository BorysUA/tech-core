using System;
using System.Globalization;
using UnityEngine;

namespace _Project.CodeBase.Data.Presets
{
  public class EventPreset : ScriptableObject
  {
    public bool Enabled = true;
    public string StartUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
    public string EndUtc = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
  }
}