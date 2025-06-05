using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.HUD.Notification
{
  public readonly struct ToastData
  {
    public string Text { get; }
    public Color? TextColor { get; }
    public Sprite Icon { get; }
    public float Duration { get; }
    public MessageStyle Style { get; }

    public ToastData(string text, Color? textColor = null, Sprite icon = null, float duration = 2f,
      MessageStyle style = MessageStyle.None)
    {
      Text = text;
      TextColor = textColor;
      Icon = icon;
      Duration = duration;
      Style = style;
    }
  }
}