using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Buildings.Modules.SelfDestruction;

namespace _Project.CodeBase.Gameplay.Buildings.Actions
{
  public class SelfDestructionAction : IBuildingAction
  {
    private SelfDestructionModule _selfDestructionModule;
    public ActionType Type => ActionType.Destroy;

    public void Setup(SelfDestructionModule selfDestructionModule)
    {
      _selfDestructionModule = selfDestructionModule;
    }

    public void Execute() =>
      _selfDestructionModule.SelfDestruct();
  }
}