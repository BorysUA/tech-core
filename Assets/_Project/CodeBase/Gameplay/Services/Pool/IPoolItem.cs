using R3;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public interface IPoolItem
  {
    Observable<Unit> Deactivated { get; }
    void Activate();
  }
}