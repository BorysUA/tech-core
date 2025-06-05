namespace _Project.CodeBase.Gameplay.Building.Actions.Common
{
  public interface IBuildingAction
  {
    ActionType Type { get; }
    void Execute();
  }
}