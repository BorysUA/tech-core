using System;
using System.Collections.Generic;
using R3;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public class ObjectPool<T> : IDisposable where T : class, IPoolItem
  {
    private readonly Queue<IPoolItem> _pool = new();
    private readonly CompositeDisposable _disposable = new();

    public void Add(T item) =>
      item.Deactivated
        .Subscribe(_ =>
        {
          _pool.Enqueue(item);
          
          if (item is IResettablePoolItem resettableItem)
            resettableItem.Reset();
        })
        .AddTo(_disposable);

    public bool TryGet(out T item)
    {
      if (_pool.Count > 0)
      {
        item = _pool.Dequeue() as T;
        item?.Activate();
        return true;
      }

      item = null;
      return false;
    }

    public void Dispose() =>
      _disposable.Dispose();
  }
}