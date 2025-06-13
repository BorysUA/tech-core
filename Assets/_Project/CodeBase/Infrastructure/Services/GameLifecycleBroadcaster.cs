using _Project.CodeBase.Gameplay.Signals.System;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class GameLifecycleBroadcaster : MonoBehaviour
  {
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) =>
      _signalBus = signalBus;

    private void Start() =>
      _signalBus.Fire<GameSessionStarted>();

    private void OnApplicationPause(bool pauseStatus) =>
      _signalBus.Fire(new GameSessionPaused(pauseStatus));

    private void OnApplicationQuit() =>
      _signalBus.Fire<GameSessionEnded>();
  }
}