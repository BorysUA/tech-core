using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Data.Progress
{
  [Serializable]
  public class GameStateData
  {
    public SessionInfo SessionInfo;
    public Dictionary<int, BuildingData> Buildings = new();
    public List<ConstructionPlotData> ConstructionPlots = new();
    public Dictionary<ResourceKind, GameResourceData> Resources;
    public Dictionary<int, ResourceDropData> ResourceDrops = new();

    public GameStateData(SessionInfo sessionInfo, Dictionary<ResourceKind, GameResourceData> resources)
    {
      Resources = resources;
      SessionInfo = sessionInfo;
    }
  }
}