using System;
using R3;

namespace _Project.CodeBase.Services.Timer
{
  public interface ITimer : IDisposable
  {
    bool IsRunning { get; }
    
    void Start();
    
    void Pause();

    void Reset();
    
    ReadOnlyReactiveProperty<float> ElapsedSeconds { get; }
  }
}