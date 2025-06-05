using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.SelfDestruction;

namespace _Project.CodeBase.Gameplay.Building.Actions
{
  public class SelfDestructionAction : IBuildingAction
  {
    private SelfDestructionModule _selfDestructionModule;
    public ActionType Type => ActionType.Destroy;

    public void Setup(SelfDestructionModule selfDestructionModule)
    {
      _selfDestructionModule = selfDestructionModule;
    }

    public void Execute()
    {
      _selfDestructionModule.SelfDestruct();
    }
  }
}