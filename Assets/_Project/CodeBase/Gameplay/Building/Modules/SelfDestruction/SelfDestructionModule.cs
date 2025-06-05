using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.Resource;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building.Modules.SelfDestruction
{
  public class SelfDestructionModule : BuildingModule, IBuildingActionsProvider
  {
    private readonly IBuildingService _buildingService;
    private readonly ActionFactory _actionFactory;
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly IResourceService _resourceService;

    private float _refundRatio;
    private ResourceCostConfig _buildingPrice;

    public IEnumerable<IBuildingAction> Actions => _actions;

    public SelfDestructionModule(IBuildingService buildingService, ActionFactory actionFactory,
      IResourceService resourceService)
    {
      _buildingService = buildingService;
      _actionFactory = actionFactory;
      _resourceService = resourceService;

      InitializeActions();
    }

    public void Setup(float refundRatio, ResourceCostConfig buildingPrice)
    {
      _refundRatio = refundRatio;
      _buildingPrice = buildingPrice;
    }

    public void SelfDestruct()
    {
      _resourceService.AddResource(_buildingPrice.Resource.Kind,
        Mathf.RoundToInt(_buildingPrice.Amount * _refundRatio));
      _buildingService.DestroyBuilding(BuildingId);
    }

    private void InitializeActions()
    {
      SelfDestructionAction selfDestructionAction = _actionFactory.CreateAction<SelfDestructionAction>();
      selfDestructionAction.Setup(this);

      _actions[0] = selfDestructionAction;
    }
  }
}