using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public abstract class MappedObjectPool<TKey, TValue> where TValue : class, IPoolItem
  {
    private readonly Dictionary<TKey, ObjectPool<TValue>> _pools = new();

    public bool TryGet(TKey type, out TValue viewModel)
    {
      if (GetOrCreatePool(type).TryGet(out viewModel))
        return true;

      viewModel = null;
      return false;
    }

    public void Add(TKey type, TValue viewModel) =>
      GetOrCreatePool(type).Add(viewModel);

    private ObjectPool<TValue> GetOrCreatePool(TKey type)
    {
      if (!_pools.ContainsKey(type))
        _pools.Add(type, new ObjectPool<TValue>());
      return _pools[type];
    }
  }
}