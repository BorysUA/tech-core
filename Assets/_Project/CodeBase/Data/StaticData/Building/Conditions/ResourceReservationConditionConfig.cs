using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Resource;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "ResourceReservationCondition",
    menuName = "ScriptableObjects/OperationalConditions/ResourceReservationCondition", order = 0)]
  public class ResourceReservationConditionConfig : OperationalConditionConfig
  {
    public BuildingIndicatorType IndicatorType;
    public ResourceAmountData ResourceToReserve;

    public override bool IsValidFor(IConditionBoundModule module) => module is ResourceReservationModule;

    public override OperationalCondition CreateOperationalCondition(Func<Type, OperationalCondition> instantiator,
      IConditionBoundModule module)
    {
      ResourceReservationModule reservationModule = (ResourceReservationModule)module;

      ResourceReservationCondition condition =
        (ResourceReservationCondition)instantiator.Invoke(typeof(ResourceReservationCondition));

      condition.Setup(IndicatorType, ResourceToReserve, reservationModule.IsReserved);

      return condition;
    }
  }
}