using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Services.Resource;

namespace _Project.CodeBase.Gameplay.Building.Actions
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
      // if (_resourceService.TrySpend())
      // {
      //   _healthModule.Repair();
      // }
    }
  }
}