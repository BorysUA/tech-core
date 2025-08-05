namespace _Project.CodeBase.Infrastructure.Signals
{
  public readonly struct AppLifecycleChanged 
  {
    public readonly Phase Current;

    public AppLifecycleChanged(Phase current)
    {
      Current = current;
    }

    public enum Phase
    {
      None = 0,
      Started = 1,
      Paused = 2,
      Resumed = 3,
      Quited = 4
    }
  }
}