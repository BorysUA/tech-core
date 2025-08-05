using System.Collections.Generic;
using R3;

namespace _Project.CodeBase.Infrastructure.ReactiveCore
{
  public class SyncLatestReactiveProperty<T> : ReactiveProperty<T>
  {
    private bool _emitting;
    private bool _dirty;
    private T _last;

    public SyncLatestReactiveProperty()
    {
    }

    public SyncLatestReactiveProperty(T value)
      : base(value)
    {
    }

    public SyncLatestReactiveProperty(T value, IEqualityComparer<T> equalityComparer)
      : base(value, equalityComparer)
    {
    }

    protected override void OnNextCore(T value)
    {
      _last = value;

      if (_emitting)
      {
        _dirty = true;
        return;
      }

      _emitting = true;
      try
      {
        base.OnNextCore(_last);

        while (_dirty)
        {
          _dirty = false;
          base.OnNextCore(_last);
        }
      }
      finally
      {
        _emitting = false;
      }
    }
  }
}