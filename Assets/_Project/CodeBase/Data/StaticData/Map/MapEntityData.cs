using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Markers.Baked;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Map
{
  [Serializable]
  public class MapEntityData
  {
    [field: SerializeField] public MapEntityType Type { get; private set; }
    [field: SerializeField] public List<Vector2Int> OccupiedCells { get; private set; }
    [field: SerializeReference] public IMapEntityPayload Payload { get; private set; }

    public MapEntityData(MapEntityType type, List<Vector2Int> occupiedCells, IMapEntityPayload payload)
    {
      Type = type;
      OccupiedCells = occupiedCells;
      Payload = payload;
    }
  }
}