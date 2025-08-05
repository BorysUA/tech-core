using ObservableCollections;

namespace _Project.CodeBase.Gameplay.Models
{
  public class WritableListProjection<TInner, TView> : ReadOnlyListProjection<TInner, TView> where TInner : TView
  {
    public WritableListProjection(ObservableList<TInner> source) : base(source)
    {
    }

    public void Add(TInner value) =>
      _source.Add(value);

    public void Remove(TInner value) =>
      _source.Remove(value);
  }
}