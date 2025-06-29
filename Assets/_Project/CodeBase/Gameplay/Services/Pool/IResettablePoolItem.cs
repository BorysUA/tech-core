namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public interface IResettablePoolItem<in TParam> : IPoolItem<TParam>
  {
    void Reset();
  }
}