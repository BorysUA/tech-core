namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public readonly struct IndicatorVisibility
  {
    public BuildingIndicatorType Type { get; }
    public bool Visible { get; }

    public IndicatorVisibility(BuildingIndicatorType type, bool visible)
    {
      Type = type;
      Visible = visible;
    }
  }
}