using _Project.CodeBase.Data.Remote;
using _Project.CodeBase.Gameplay.Meteorite;

namespace _Project.CodeBase.Gameplay.LiveEvents
{
  public class MeteorShowerEvent : GameEventBase
  {
    private readonly MeteoriteSpawner _meteoriteSpawner;
    private MeteorShowerEventData _eventData;

    public override GameEventType Type => GameEventType.MeteorShowerEvent;

    public MeteorShowerEvent(MeteoriteSpawner meteoriteSpawner)
    {
      _meteoriteSpawner = meteoriteSpawner;
    }

    public override void Initialize(BaseEventData eventData)
    {
      base.Initialize(eventData);

      _eventData = eventData as MeteorShowerEventData;
      _meteoriteSpawner.Initialize(_eventData!.Multiplier);
    }

    protected override void OnActivate() =>
      _meteoriteSpawner.Start();

    protected override void OnDeactivate() =>
      _meteoriteSpawner.Stop();
  }
}