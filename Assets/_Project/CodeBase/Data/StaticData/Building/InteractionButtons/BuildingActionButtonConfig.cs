using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.CodeBase.Data.StaticData.Building.InteractionButtons
{
  [CreateAssetMenu(fileName = "BuildingInteractionButton",
    menuName = "ScriptableObjects/BuildingInteractionButton", order = 0)]
  public class BuildingActionButtonConfig : ScriptableObject
  {
    public ActionType Type;
    public string Title;
    public AssetReferenceAtlasedSprite Icon;
  }
}