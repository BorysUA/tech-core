namespace _Project.CodeBase.Services.TimeCounter
{
  public interface ITimerFactory
  {
    public ITimer Create(bool autoStart = true);
    public void DisposeTimer(ITimer timer);
  }
}