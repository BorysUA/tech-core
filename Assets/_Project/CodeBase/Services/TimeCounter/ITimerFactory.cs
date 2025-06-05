namespace _Project.CodeBase.Services.Timer
{
  public interface ITimerFactory
  {
    public ITimer Create(bool autoStart = true);
    public void DisposeTimer(ITimer timer);
  }
}