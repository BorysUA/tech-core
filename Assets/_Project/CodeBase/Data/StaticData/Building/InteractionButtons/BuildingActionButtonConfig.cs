using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building.InteractionButtons
{
  [CreateAssetMenu(fileName = "BuildingInteractionButton",
    menuName = "ScriptableObjects/BuildingInteractionButton", order = 0)]
  public class BuildingActionButtonConfig : ScriptableObject
  {
    public ActionType Type;
    public string Title;
    public Sprite Icon;
  }
}