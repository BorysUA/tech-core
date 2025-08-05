using System.Collections.Generic;
using ObservableCollections;
using R3;

namespace _Project.CodeBase.Gameplay.Models
{
  public class ReadOnlyListProjection<TInner, TView> where TInner : TView
  {
    private readonly ISynchronizedView<TInner, TView> _view;
    protected readonly ObservableList<TInner> _source;

    public int Count => _view.Count;
    public TView this[int index] => _source[index];
    public IReadOnlyCollection<TView> Values => _view;

    public ReadOnlyListProjection(ObservableList<TInner> source)
    {
      _source = source;
      _view = source.CreateView(item => (TView)item);
    }

    public Observable<CollectionAddEvent<TView>> ObserveAdd() => _source
      .ObserveAdd()
      .Select(addEvent => new CollectionAddEvent<TView>(addEvent.Index, addEvent.Value));

    public Observable<CollectionRemoveEvent<TView>> ObserveRemove() => _source
      .ObserveRemove()
      .Select(removeEvent => new CollectionRemoveEvent<TView>(removeEvent.Index, removeEvent.Value));

    public Observable<CollectionReplaceEvent<TView>> ObserveReplace() => _source
      .ObserveReplace()
      .Select(replaceEvent => new CollectionReplaceEvent<TView>(
        replaceEvent.Index,
        replaceEvent.OldValue,
        replaceEvent.NewValue));

    public Observable<CollectionResetEvent<TView>> ObserveReset() => _source
      .ObserveReset()
      .Select(_ => new CollectionResetEvent<TView>());
  }
}