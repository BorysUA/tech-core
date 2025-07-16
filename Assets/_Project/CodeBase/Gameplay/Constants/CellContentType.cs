using System;

namespace _Project.CodeBase.Gameplay.Constants
{
  [Flags]
  public enum CellContentType
  {
    None = 0,
    ConstructionPlot = 1 << 1,
    ResourceSpot = 1 << 2,
    Building = 1 << 3,
    Obstacle = 1 << 4,
  }
}