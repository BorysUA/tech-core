using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Buildings.Actions;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Services.Buildings;
using _Project.CodeBase.Gameplay.Services.Resource;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.SelfDestruction
{
  public class SelfDestructionModule : BuildingModule, IBuildingActionsProvider
  {
    private readonly IBuildingService _buildingService;
    private readonly ActionFactory _actionFactory;
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly IResourceService _resourceService;

    private float _refundRatio;
    private ResourceAmountData _buildingPrice;

    public IReadOnlyList<IBuildingAction> Actions => _actions;

    public SelfDestructionModule(IBuildingService buildingService, ActionFactory actionFactory,
      IResourceService resourceService)
    {
      _buildingService = buildingService;
      _actionFactory = actionFactory;
      _resourceService = resourceService;
    }

    public void Setup(float refundRatio, ResourceAmountData buildingPrice)
    {
      _refundRatio = refundRatio;
      _buildingPrice = buildingPrice;
    }

    protected override void OnInitialize()
    {
      CreateActions();
    }

    public void SelfDestruct()
    {
      _resourceService.AddResource(_buildingPrice.Kind,
        Mathf.RoundToInt(_buildingPrice.Amount * _refundRatio));
      _buildingService.DestroyBuilding(BuildingId);
    }

    private void CreateActions()
    {
      SelfDestructionAction selfDestructionAction = _actionFactory.CreateAction<SelfDestructionAction>();
      selfDestructionAction.Setup(this);

      _actions[0] = selfDestructionAction;
    }
  }
}