namespace _Project.CodeBase.Gameplay.Buildings.Actions.Common
{
  public interface IBuildingAction
  {
    ActionType Type { get; }
    void Execute();
  }
}