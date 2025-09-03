using System;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.TimeCounter;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Timers
{
  public class SessionTimer : IDisposable, IGameplayInit, IGameplayStartedListener, IGameplayPausedListener,
    ISessionTimer
  {
    private const int PlaytimeSaveInterval = 10;

    private readonly ITimerFactory _timerFactory;
    private readonly ICommandBroker _commandBroker;
    private readonly CompositeDisposable _subscriptions = new();
    private readonly ReactiveProperty<float> _sessionPlaytime = new(0);
    private ITimer _playTimer;

    private float _lastSavedPlaytime;

    public ReadOnlyReactiveProperty<float> SessionPlaytime => _sessionPlaytime;
    public InitPhase InitPhase => InitPhase.Preparation;

    public SessionTimer(ITimerFactory timerFactory, ICommandBroker commandBroker)
    {
      _timerFactory = timerFactory;
      _commandBroker = commandBroker;
    }

    public void Initialize()
    {
      _playTimer = _timerFactory.Create(autoStart: false);

      _playTimer.ElapsedSeconds
        .Subscribe(sessionTime => _sessionPlaytime.Value = sessionTime)
        .AddTo(_subscriptions);

      Observable
        .Interval(TimeSpan.FromSeconds(PlaytimeSaveInterval))
        .Subscribe(_ =>
        {
          float delta = _sessionPlaytime.CurrentValue - _lastSavedPlaytime;
          _lastSavedPlaytime = _sessionPlaytime.CurrentValue;

          UpdateSessionPlaytimeCommand command = new UpdateSessionPlaytimeCommand(delta);
          _commandBroker.ExecuteCommand(command);
        })
        .AddTo(_subscriptions);
    }

    public void OnGameplayStarted() =>
      _playTimer.Start();

    public void OnGameplayPaused() =>
      _playTimer.Pause();

    public void OnGameplayResumed() =>
      _playTimer.Start();

    public void Dispose() =>
      _subscriptions.Dispose();
  }
}