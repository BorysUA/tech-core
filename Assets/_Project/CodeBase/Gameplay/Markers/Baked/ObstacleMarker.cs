using _Project.CodeBase.Gameplay.Markers.Baked.Payloads;

namespace _Project.CodeBase.Gameplay.Markers.Baked
{
  public class ObstacleMarker : MapEntityMarker<EmptyPayload>
  {
    protected override MapEntityType EntityType => MapEntityType.Obstacle;

#if UNITY_EDITOR
    protected override EmptyPayload FillPayload() =>
      EmptyPayload.Instance;
#endif
  }
}