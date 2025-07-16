using _Project.CodeBase.Gameplay.Markers.Baked.Payloads;

namespace _Project.CodeBase.Gameplay.Markers.Baked
{
  public class ObstacleMarker : MapEntityMarker<EmptyPayload>
  {
#if UNITY_EDITOR
    protected override MapEntityType EntityType => MapEntityType.Obstacle;

    protected override EmptyPayload FillPayload() =>
      EmptyPayload.Instance;
#endif
  }
}