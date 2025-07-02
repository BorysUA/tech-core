using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Markers.Baked.Payloads
{
  public class ResourceSpotData : IMapEntityPayload
  {
    public ResourceKind Kind { get; }

    public ResourceSpotData(ResourceKind kind)
    {
      Kind = kind;
    }
  }
}