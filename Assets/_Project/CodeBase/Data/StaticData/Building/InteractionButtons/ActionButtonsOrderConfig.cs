using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.InteractionButtons
{
  [CreateAssetMenu(fileName = "ActionButtonsOrder",
    menuName = "ScriptableObjects/ActionButtonsOrder", order = 0)]
  public class ActionButtonsOrderConfig : ScriptableObject
  {
    public ActionType[] Order;
  }
}