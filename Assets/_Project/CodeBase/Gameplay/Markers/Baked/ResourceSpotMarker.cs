using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Markers.Baked.Payloads;
using _Project.CodeBase.Utility;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Markers.Baked
{
  public class ResourceSpotMarker : MapEntityMarker<ResourceSpotData>
  {
    public ResourceKind Kind;

#if UNITY_EDITOR
    protected override MapEntityType EntityType => MapEntityType.ResourceSpot;

    protected override ResourceSpotData FillPayload() =>
      new(Kind);

    public override void OnDrawGizmos()
    {
      Gizmos.color = Kind switch
      {
        ResourceKind.Metal => Color.gray,
        ResourceKind.Energy => Color.yellow,
        _ => Color.magenta
      };

      Gizmos.DrawCube(GizmoUtils.Lift(transform.position), new Vector3(SizeInCells.x, 0, SizeInCells.y));
    }
#endif
  }
}