using System;
using R3;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Services.Timer
{
  public class Timer : ITimer
  {
    private readonly ReactiveProperty<float> _elapsedSeconds = new(0f);
    public bool IsRunning { get; private set; }
    public ReadOnlyReactiveProperty<float> ElapsedSeconds => _elapsedSeconds;

    public void Start() => IsRunning = true;
    public void Pause() => IsRunning = false;

    public void Reset()
    {
      IsRunning = false;
      _elapsedSeconds.Value = 0f;
    }

    public void Tick()
    {
      if (!IsRunning)
        return;

      _elapsedSeconds.Value += Time.deltaTime;
    }

    public void Dispose() => _elapsedSeconds.Dispose();
  }
}