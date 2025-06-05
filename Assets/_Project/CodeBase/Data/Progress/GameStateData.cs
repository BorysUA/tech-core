using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Gameplay.Constants;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.CodeBase.Data.Progress
{
  [Serializable]
  public class GameStateData
  {
    public SessionInfo SessionInfo;
    public Dictionary<string, BuildingData> Buildings = new();
    public List<ConstructionPlotData> ConstructionPlots = new();
    public Dictionary<ResourceKind, GameResourceData> Resources;
    public Dictionary<string, ResourceDropData> ResourceDrops = new();

    public GameStateData(SessionInfo sessionInfo, Dictionary<ResourceKind, GameResourceData> resources)
    {
      Resources = new Dictionary<ResourceKind, GameResourceData>(resources);
      SessionInfo = sessionInfo;
    }
  }
}