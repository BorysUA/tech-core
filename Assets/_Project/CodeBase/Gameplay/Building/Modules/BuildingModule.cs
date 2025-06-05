using System;
using R3;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public abstract class BuildingModule
  {
    private readonly ReactiveProperty<bool> _isActive = new();

    public ReadOnlyReactiveProperty<bool> IsActive => _isActive;
    protected string BuildingId { get; private set; }

    public void Setup(string buildingId)
    {
      BuildingId = buildingId;
    }

    public virtual void Initialize()
    {
    }

    public virtual void Activate()
    {
      _isActive.Value = true;
    }

    public virtual void Deactivate()
    {
      _isActive.Value = false;
    }

    public virtual void OnSelected()
    {
    }

    public virtual void OnUnselected()
    {
    }

    public virtual void OnDestroyed()
    {
    }
  }
}