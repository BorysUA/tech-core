using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Map;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Utility;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Markers.Baked
{
  public abstract class MapEntityMarker<TPayload> : BaseMarker where TPayload : IMapEntityPayload
  {
    public Vector2Int SizeInCells;
    protected abstract MapEntityType EntityType { get; }

#if UNITY_EDITOR
    public override MapEntityData Bake()
    {
      Vector3 snapped = GridUtils.GetSnappedPosition(transform.position, SizeInCells);
      transform.position = snapped;

      List<Vector2Int> cells = GridUtils.GetCells(snapped, SizeInCells);

      return new MapEntityData(EntityType, cells, FillPayload());
    }

    protected abstract TPayload FillPayload();

    public virtual void OnDrawGizmos()
    {
      Gizmos.color = Color.orange;
      Gizmos.DrawCube(GizmoUtils.Lift(transform.position), new Vector3(SizeInCells.x, 0, SizeInCells.y));
    }
#endif
  }
}