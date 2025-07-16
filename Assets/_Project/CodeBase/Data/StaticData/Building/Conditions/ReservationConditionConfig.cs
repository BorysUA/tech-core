using System;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.Conditions
{
  [CreateAssetMenu(fileName = "ResourceReservationCondition",
    menuName = "ScriptableObjects/OperationalConditions/ResourceReservationCondition", order = 0)]
  public class ReservationConditionConfig : OperationalConditionConfig
  {
    public ResourceAmountData ResourceToReserve;

    protected override OperationalCondition InstantiateCondition(Func<Type, OperationalCondition> instantiator) =>
      instantiator.Invoke(typeof(ResourceReservationCondition));

    public override bool IsValidFor(BuildingModule module) => true;

    protected override void SetupCondition(OperationalCondition condition, BuildingModule module)
    {
      base.SetupCondition(condition, module);
      ResourceReservationCondition resourceReservationCondition = (ResourceReservationCondition)condition;
      resourceReservationCondition.Setup(ResourceToReserve);
    }
  }
}