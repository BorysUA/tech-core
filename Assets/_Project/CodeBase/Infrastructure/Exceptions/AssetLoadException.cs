using System;

namespace _Project.CodeBase.Infrastructure.Exceptions
{
  public class AssetLoadException : Exception
  {
    public string AssetKey { get; }

    public AssetLoadException(string message)
      : base(message) { }

    public AssetLoadException(string message, Exception inner)
      : base(message, inner) { }

    public AssetLoadException(string message, string assetKey)
      : base(message)
    {
      AssetKey = assetKey;
    }
  }
}