using UnityEngine;

namespace _Project.CodeBase.Utility
{
  static class GizmoUtils
  {
    private const float Eps = 0.05f;
    public static Vector3 Lift(Vector3 pos) => pos + Vector3.up * Eps;
  }
}