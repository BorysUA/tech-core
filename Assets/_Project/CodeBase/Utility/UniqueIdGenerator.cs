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

    public static int GenerateUniqueIntId()
    {
      Guid guid = Guid.NewGuid();
      byte[] bytes = guid.ToByteArray();
      return (int)(BitConverter.ToUInt32(bytes, 0) & 0x7FFFFFFF);
    }
  }
}