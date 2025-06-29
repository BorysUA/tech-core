using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Services.Pool
{
  public abstract class MappedObjectPool<TKey, TValue, TParam> where TValue : class, IPoolItem<TParam>
  {
    private readonly Dictionary<TKey, ObjectPool<TValue, TParam>> _pools = new();

    public bool TryGet(TKey type, TParam param, out TValue viewModel)
    {
      if (GetOrCreatePool(type).TryGet(param, out viewModel))
        return true;

      viewModel = null;
      return false;
    }

    public void Add(TKey type, TValue viewModel) =>
      GetOrCreatePool(type).Add(viewModel);

    private ObjectPool<TValue, TParam> GetOrCreatePool(TKey type)
    {
      if (!_pools.ContainsKey(type))
        _pools.Add(type, new ObjectPool<TValue, TParam>());
      return _pools[type];
    }
  }

  public abstract class MappedObjectPool<TKey, TValue> : MappedObjectPool<TKey, TValue, PoolUnit>
    where TValue : class, IPoolItem<PoolUnit>
  {
    public bool TryGet(TKey type, out TValue viewModel) =>
      base.TryGet(type, PoolUnit.Default, out viewModel);
  }
}