using UnityEngine;

namespace _Project.CodeBase.Extensions
{
  public static class VectorExtensions
  {
    public static Vector3 ToXZ(this Vector3 vector) => 
      new(vector.x, 0, vector.z);
  }
}