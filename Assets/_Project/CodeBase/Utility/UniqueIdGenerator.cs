using System;

namespace _Project.CodeBase.Utility
{
  public static class UniqueIdGenerator
  {
    public static Guid GenerateGuid()
    {
      return Guid.NewGuid();
    }

    public static string GenerateUniqueStringId()
    {
      return Guid.NewGuid().ToString("N");
    }
  }
}