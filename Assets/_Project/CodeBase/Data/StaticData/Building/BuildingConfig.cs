using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.CodeBase.Data.StaticData.Building
{
  [CreateAssetMenu(fileName = "BuildingConfig", menuName = "ScriptableObjects/BuildingConfig", order = 0)]
  public class BuildingConfig : ScriptableObject
  {
    public BuildingType Type;
    public BuildingCategory Category;
    public BuildingModuleConfig[] BuildingsModules;
    public AssetReferenceGameObject PrefabReference;
    public AssetReferenceT<Mesh> MeshReference;
    public Vector2Int SizeInCells;
    public ResourceAmountData Price;
    public string Title;
    public Sprite Icon;
    public int StartLevel;
    public int MaxLevel;

    [Header("Requirements for building construction")]
    public PlacementFilter PlacementFilter;
  }
}