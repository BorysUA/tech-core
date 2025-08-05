using System;
using R3;

namespace _Project.CodeBase.Infrastructure.ReactiveCore
{
  public class SyncLatestReadOnlyReactiveProperty<T> : ReadOnlyReactiveProperty<T>
  {
    private readonly SyncLatestReactiveProperty<T> _inner;
    private readonly IDisposable _subscription;

    public override T CurrentValue => _inner.CurrentValue;

    public SyncLatestReadOnlyReactiveProperty(Observable<T> source, T initialValue)
    {
      _inner = new SyncLatestReactiveProperty<T>(initialValue);
      _subscription = source.Subscribe(value => _inner.Value = value);
    }

    protected override IDisposable SubscribeCore(Observer<T> observer)
    {
      return _inner.Subscribe(
        observer.OnNext,
        observer.OnErrorResume,
        observer.OnCompleted
      );
    }

    public override void Dispose()
    {
      _subscription.Dispose();
      _inner.Dispose();
    }
  }
}