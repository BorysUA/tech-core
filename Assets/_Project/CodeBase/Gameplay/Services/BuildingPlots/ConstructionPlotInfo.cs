using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public readonly struct ConstructionPlotInfo
  {
    public ConstructionPlotType Type { get; }
    public Vector2Int Size { get; }

    public ConstructionPlotInfo(ConstructionPlotType type, Vector2Int size)
    {
      Type = type;
      Size = size;
    }
  }
}