namespace _Project.CodeBase.Gameplay.Signals.System
{
  public readonly struct GameSessionPaused
  {
    public bool Status { get; private init; }

    public GameSessionPaused(bool status)
    {
      Status = status;
    }
  }
}