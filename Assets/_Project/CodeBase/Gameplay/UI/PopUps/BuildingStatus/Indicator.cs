using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public class Indicator
  {
    public IndicatorSlotType SlotType { get; }
    public BuildingIndicatorType Type { get; }
    public ReadOnlyReactiveProperty<bool> IsShown { get; }
    public int Priority { get; }

    public Indicator(IndicatorSlotType slotType, BuildingIndicatorType type, ReadOnlyReactiveProperty<bool> isShown,
      int priority)
    {
      SlotType = slotType;
      Type = type;
      IsShown = isShown;
      Priority = priority;
    }
  }
}