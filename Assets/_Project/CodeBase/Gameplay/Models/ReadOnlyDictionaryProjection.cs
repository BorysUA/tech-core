using System.Collections.Generic;
using ObservableCollections;
using R3;

namespace _Project.CodeBase.Gameplay.Models
{
  public class ReadOnlyDictionaryProjection<TKey, TInner, TView> where TInner : TView
  {
    private readonly ISynchronizedView<KeyValuePair<TKey, TInner>, TView> _view;
    protected readonly ObservableDictionary<TKey, TInner> _source;

    public IReadOnlyCollection<TView> Values => _view;

    public TView this[TKey key] => _source[key];

    public ReadOnlyDictionaryProjection(ObservableDictionary<TKey, TInner> source)
    {
      _source = source;
      _view = source.CreateView(pair => (TView)pair.Value);
    }

    public bool ContainsKey(TKey key) =>
      _source.ContainsKey(key);

    public Observable<CollectionAddEvent<TView>> ObserveAdd() => _source
      .ObserveAdd()
      .Select(addEvent => new CollectionAddEvent<TView>(addEvent.Index, addEvent.Value.Value));

    public Observable<CollectionRemoveEvent<TView>> ObserveRemove() => _source
      .ObserveRemove()
      .Select(removeEvent => new CollectionRemoveEvent<TView>(removeEvent.Index, removeEvent.Value.Value));

    public Observable<CollectionReplaceEvent<TView>> ObserveReplace() => _source
      .ObserveReplace()
      .Select(replaceEvent => new CollectionReplaceEvent<TView>(
        replaceEvent.Index,
        replaceEvent.OldValue.Value,
        replaceEvent.NewValue.Value
      ));

    public Observable<CollectionResetEvent<TView>> ObserveReset() => _source
      .ObserveReset()
      .Select(_ => new CollectionResetEvent<TView>());

    public bool TryGetValue(TKey key, out TView value)
    {
      if (_source.TryGetValue(key, out TInner actual))
      {
        value = actual;
        return true;
      }

      value = default!;
      return false;
    }

    public TView Get(TKey key) =>
      _source.GetValueOrDefault(key);
  }
}