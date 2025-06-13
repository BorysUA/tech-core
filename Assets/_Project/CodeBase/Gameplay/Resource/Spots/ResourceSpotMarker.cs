using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Utility;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Resource.Spots
{
#if UNITY_EDITOR
  [ExecuteAlways]
#endif
  public class ResourceSpotMarker : MonoBehaviour
  {
    public ResourceKind Kind;
    public Vector2Int SizeInCells;

#if UNITY_EDITOR
    private void OnDrawGizmos()
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