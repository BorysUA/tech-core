using System;

namespace _Project.CodeBase.Extensions
{
  public static class ArrayExtensions
  {
    public static void Shuffle<T>(this T[] array, Random random = null)
    {
      random ??= new Random();

      int remainingCount = array.Length;
      while (remainingCount > 1)
      {
        remainingCount--;
        int randomIndex = random.Next(remainingCount + 1);
        (array[randomIndex], array[remainingCount]) = (array[remainingCount], array[randomIndex]);
      }
    }
  }
}