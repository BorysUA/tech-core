using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace _Project.CodeBase.Services.TimeCounter
{
  public class TimerFactory : ITimerFactory, ITickable
  {
    private readonly List<Timer> _timers = new();

    public ITimer Create(bool autoStart = true)
    {
      Timer timer = new Timer();
      if (autoStart)
        timer.Start();

      _timers.Add(timer);
      return timer;
    }

    public void DisposeTimer(ITimer timer)
    {
      Timer timerToDispose = _timers.FirstOrDefault(t => t == timer);
      
      if (timerToDispose != null)
      {
        _timers.Remove(timerToDispose);
        timerToDispose.Dispose();
      }
    }

    public void Tick()
    {
      foreach (Timer timer in _timers) 
        timer.Tick();
    }
  }
}