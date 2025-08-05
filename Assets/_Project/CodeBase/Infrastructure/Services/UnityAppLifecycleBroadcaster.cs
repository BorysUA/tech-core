using _Project.CodeBase.Infrastructure.Signals;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class UnityAppLifecycleBroadcaster : MonoBehaviour
  {
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus) =>
      _signalBus = signalBus;

    private void Awake() =>
      _signalBus.Fire(new AppLifecycleChanged(AppLifecycleChanged.Phase.Started));

    private void OnApplicationPause(bool pauseStatus)
    {
      _signalBus.Fire(pauseStatus
        ? new AppLifecycleChanged(AppLifecycleChanged.Phase.Paused)
        : new AppLifecycleChanged(AppLifecycleChanged.Phase.Resumed));
    }

    private void OnApplicationQuit() =>
      _signalBus.Fire(new AppLifecycleChanged(AppLifecycleChanged.Phase.Quited));
  }
}