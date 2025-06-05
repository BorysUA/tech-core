using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Resource.Spots;
using _Project.CodeBase.Gameplay.Services.Grid;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
namespace _Project.CodeBase.EditorTools
{
  public static class ResourceSpotBaker
  {
    private const string ResourceSpotMapPass = "Assets/_Project/Bundles/Configs/ResourceSpotsConfig.asset";

    [MenuItem("Tools/Rebake resource spots")]
    public static void Bake()
    {
      GridService gridService = new();

      ResourceSpotMarker[] markers = Object.FindObjectsByType<ResourceSpotMarker>(FindObjectsSortMode.None);
      List<ResourceSpotEntry> spots = new List<ResourceSpotEntry>();

      foreach (ResourceSpotMarker marker in markers)
      {
        Vector3 snapped = gridService.GetSnappedPosition(marker.transform.position, marker.SizeInCells);
        marker.transform.position = snapped;

        List<Vector2Int> cells = gridService.GetCells(snapped, marker.SizeInCells);
        spots.Add(new ResourceSpotEntry(marker.Kind, cells));
      }

      ResourceSpotMap map = AssetDatabase.LoadAssetAtPath<ResourceSpotMap>(ResourceSpotMapPass);
      map.Spots = spots.ToArray();
      EditorUtility.SetDirty(map);
      AssetDatabase.SaveAssets();
      
      Debug.Log($"ResourceSpotBaker: Baked {spots.Count} spots to {map.name}");
    }
  }
}
#endif