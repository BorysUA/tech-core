using R3;

namespace _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus
{
  public interface IBuildingIndicatorSource
  {
    public BuildingIndicatorType Type { get; }
    public ReadOnlyReactiveProperty<bool> IsVisible { get; }
  }
}