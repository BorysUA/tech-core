using System;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.TimeCounter;
using R3;

namespace _Project.CodeBase.Gameplay.Services
{
  public class SessionTimer : ISessionTimer, IDisposable, IGameplayInit
  {
    private readonly ITimerFactory _timerFactory;
    private readonly IProgressService _progressService;
    private readonly CompositeDisposable _disposable = new();
    private ITimer _playTimer;

    public SessionTimer(ITimerFactory timerFactory, IProgressService progressService)
    {
      _timerFactory = timerFactory;
      _progressService = progressService;
    }

    public void Initialize()
    {
      SessionInfoProxy sessionInfo = _progressService.GameStateProxy.SessionInfo;
      float savedTotalTime = sessionInfo.TotalPlayTime.Value;

      _playTimer = _timerFactory.Create(autoStart: false);

      _playTimer.ElapsedSeconds
        .Subscribe(sessionTime =>
        {
          sessionInfo.SessionPlayTime.Value = sessionTime;
          sessionInfo.TotalPlayTime.Value = savedTotalTime + sessionTime;
        })
        .AddTo(_disposable);
    }

    public void Start() => _playTimer.Start();

    public void Pause() => _playTimer.Pause();

    public void Dispose()
    {
      _disposable.Dispose();
    }
  }
}