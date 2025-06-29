using System;
using System.Collections.Generic;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public class ObjectPool<TItem, TParam> : IDisposable where TItem : class, IPoolItem<TParam>
  {
    private readonly Queue<IPoolItem<TParam>> _pool = new();
    private readonly CompositeDisposable _disposable = new();

    public void Add(TItem item) =>
      item.Deactivated
        .Subscribe(_ =>
        {
          _pool.Enqueue(item);

          if (item is IResettablePoolItem<TParam> resettableItem)
            resettableItem.Reset();
        })
        .AddTo(_disposable);

    public bool TryGet(TParam param, out TItem item)
    {
      if (_pool.Count > 0)
      {
        item = _pool.Dequeue() as TItem;
        item?.Activate(param);
        return true;
      }

      item = null;
      return false;
    }

    public void Dispose() =>
      _disposable.Dispose();
  }

  public class ObjectPool<TItem> : ObjectPool<TItem, PoolUnit> where TItem : class, IPoolItem<PoolUnit>
  {
    public bool TryGet(out TItem item) =>
      base.TryGet(PoolUnit.Default, out item);
  }
}