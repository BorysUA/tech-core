using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Utility;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
namespace _Project.CodeBase.Gameplay.Resource.Spots
{
  [ExecuteAlways]
  public class ResourceSpotMarker : MonoBehaviour
  {
    public ResourceKind Kind;
    public Vector2Int SizeInCells;

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
  }
}
#endif