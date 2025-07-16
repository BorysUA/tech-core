using _Project.CodeBase.Gameplay.Services.CameraSystem;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Building.VFX
{
  public abstract class SimpleEffect : MonoBehaviour
  {
    private CameraRigAgent _cameraRigAgent;
    private bool _isPaused;

    [Inject]
    public void Construct(CameraRigAgent cameraRigAgent) =>
      _cameraRigAgent = cameraRigAgent;

    public abstract void Play();
    protected abstract void Pause();
    public abstract void Stop();

    protected virtual void Update()
    {
      bool visible = _cameraRigAgent.IsVisible(transform.position);

      if (visible)
        ResumeIfPaused();
      else
        PauseIfVisible();
    }

    private void PauseIfVisible()
    {
      if (_isPaused)
        return;

      _isPaused = true;
      Pause();
    }

    private void ResumeIfPaused()
    {
      if (!_isPaused)
        return;

      _isPaused = false;
      Play();
    }
  }
}