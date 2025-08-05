using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace _Project.CodeBase.Data.StaticData.Building
{
  [CreateAssetMenu(fileName = "ConstructionPlotConfig", menuName = "ScriptableObjects/ConstructionPlotConfig",
    order = 0)]
  public class ConstructionPlotConfig : ScriptableObject
  {
    public ConstructionPlotType Type;
    public Vector2Int SizeInCells;
    public ResourceAmountData Price;
    public AssetReferenceGameObject PrefabReference;

    [FormerlySerializedAs("RequiredContent")] [Header("Requirements for building construction")]
    public PlacementFilter PlacementFilter;

    [Header("UI")] public string Title;
    public Sprite Icon;
  }
}