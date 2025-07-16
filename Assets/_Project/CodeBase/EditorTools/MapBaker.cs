#if UNITY_EDITOR
using System.Linq;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Gameplay.Markers.Baked;
using UnityEditor;
using UnityEngine;

namespace _Project.CodeBase.EditorTools
{
  public static class MapBaker
  {
    private const string MapPath = "Assets/_Project/Bundles/Configs/GameMap.asset";

    [MenuItem("Tools/Rebake map")]
    public static void Bake()
    {
      var markers = Object.FindObjectsByType<BaseMarker>(FindObjectsSortMode.None);
      MapEntityData[] entitiesData = markers.Select(marker => marker.Bake()).ToArray();

      GameMap gameMap = AssetDatabase.LoadAssetAtPath<GameMap>(MapPath);
      gameMap.Entities = entitiesData;
      EditorUtility.SetDirty(gameMap);
      AssetDatabase.SaveAssets();

      Debug.Log($"MapBaker: Baked {entitiesData.Length} entities to {gameMap.name}");
    }
  }
}
#endif