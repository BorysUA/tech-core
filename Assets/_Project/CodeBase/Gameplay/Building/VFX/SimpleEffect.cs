using _Project.CodeBase.Gameplay.Services.CameraSystem;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Gameplay.Building.VFX
{
  public abstract class SimpleEffect : MonoBehaviour
  {
    private CameraRigAgent _cameraRigAgent;
    private bool _isPaused; 
    private bool _isActive; 

    [Inject]
    public void Construct(CameraRigAgent cameraRigAgent)
      => _cameraRigAgent = cameraRigAgent;
    
    public void Play()
    {
      _isActive = true;
      if (_cameraRigAgent.IsVisible(transform.position))
      {
        _isPaused = false;
        OnPlay();
      }
      else
      {
        _isPaused = true;
        OnPause();
      }
    }

    public void Stop()
    {
      _isActive = false; 
      _isPaused = false; 
      OnStop();
    }

    protected virtual void Update()
    {
      if (!_isActive)
        return;

      bool visible = _cameraRigAgent.IsVisible(transform.position);

      if (visible)
        ResumeIfPaused();
      else
        PauseIfVisible();
    }

    private void PauseIfVisible()
    {
      if (_isPaused) return;
      _isPaused = true;
      OnPause();
    }

    private void ResumeIfPaused()
    {
      if (!_isPaused) return;
      _isPaused = false;
      OnPlay();
    }
    
    protected abstract void OnPlay();
    protected abstract void OnPause();
    protected abstract void OnStop();
  }
}