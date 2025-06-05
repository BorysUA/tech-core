using UnityEngine;

namespace _Project.CodeBase.Utility
{
  public static class VectorUtils
  {
    public static Vector3 GetRandomXZDirection()
    {
      float angle = Random.Range(0f, Mathf.PI * 2f);
      return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
    }
    
    public static bool ApproximatelyEqual(Vector3 a, Vector3 b, float epsilon = 0.0001f)
    {
      return (a - b).sqrMagnitude < epsilon * epsilon;
    }
  }
}