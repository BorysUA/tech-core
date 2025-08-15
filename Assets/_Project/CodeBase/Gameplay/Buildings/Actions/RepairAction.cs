using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Buildings.Modules.Health;
using _Project.CodeBase.Gameplay.Services.Resource;

namespace _Project.CodeBase.Gameplay.Buildings.Actions
{
  public class RepairAction : IBuildingAction
  {
    private HealthModule _healthModule;
    
    private readonly IResourceService _resourceService;
    private readonly int _repairCost;
    
    public ActionType Type => ActionType.Repair;

    public RepairAction(IResourceService resourceService)
    {
      _resourceService = resourceService;
    }

    public void Setup(HealthModule healthModule)
    {
      _healthModule = healthModule;
    }

    public void Execute()
    {
      //TODO: repairAction
    }
  }
}