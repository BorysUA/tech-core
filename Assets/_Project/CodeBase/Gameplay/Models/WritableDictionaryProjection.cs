using ObservableCollections;

namespace _Project.CodeBase.Gameplay.Models
{
  public class WritableDictionaryProjection<TKey, TIn, TOut> : ReadOnlyDictionaryProjection<TKey, TIn, TOut>
    where TIn : TOut
  {
    public WritableDictionaryProjection(ObservableDictionary<TKey, TIn> source) : base(source)
    {
    }

    public void Add(TKey key, TIn value) =>
      _source.Add(key, value);

    public void Remove(TKey key) =>
      _source.Remove(key);
  }
}