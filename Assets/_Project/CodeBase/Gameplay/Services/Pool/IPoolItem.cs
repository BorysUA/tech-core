using R3;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public interface IPoolItem<in TParam>
  {
    Observable<Unit> Deactivated { get; }
    void Activate(TParam param);
  }
}