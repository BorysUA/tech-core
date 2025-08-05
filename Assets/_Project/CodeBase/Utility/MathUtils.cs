using System;

namespace _Project.CodeBase.Utility
{
  public static class MathUtils
  {
    public static int LerpInt(int a, int b, double t)
    {
      t = Math.Clamp(t, 0.0, 1.0);
      double v = a + (b - a) * t;
      return (int)Math.Round(v, MidpointRounding.AwayFromZero);
    }

    public static int Hash32(params int[] values)
    {
      unchecked
      {
        int hash = 5381;
        foreach (int v in values)
        {
          hash = ((hash << 5) + hash) ^ v;
        }

        return hash;
      }
    }

    public static void ShuffleFirstKIndices(int[] idx, int k, Random rng)
    {
      for (int i = 0; i < k; i++)
      {
        int j = rng.Next(i, idx.Length);
        (idx[i], idx[j]) = (idx[j], idx[i]);
      }
    }
  }
}