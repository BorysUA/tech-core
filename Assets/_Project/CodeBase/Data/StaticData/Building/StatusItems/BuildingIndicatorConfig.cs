using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building.StatusItems
{
  [CreateAssetMenu(fileName = "BuildingIndicatorConfig", menuName = "ScriptableObjects/BuildingIndicatorConfig",
    order = 0)]
  public class BuildingIndicatorConfig : ScriptableObject
  {
    public BuildingIndicatorType Type;
    public IndicatorSlotType SlotType;
    public int Priority;
    public AssetReferenceAtlasedSprite Icon;
  }
}