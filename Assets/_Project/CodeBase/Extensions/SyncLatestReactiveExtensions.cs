using _Project.CodeBase.Infrastructure.ReactiveCore;
using R3;

namespace _Project.CodeBase.Extensions
{
  public static class SyncLatestReactiveExtensions
  {
    public static ReadOnlyReactiveProperty<T> ToStabilizedReadOnlyReactiveProperty<T>(
      this Observable<T> source, T initialValue = default)
    {
      return new SyncLatestReadOnlyReactiveProperty<T>(source, initialValue);
    }
    
    public static ReadOnlyReactiveProperty<T> ToStabilizedReadOnlyReactiveProperty<T>(
      this ReadOnlyReactiveProperty<T> source, T initialValue = default)
    {
      return new SyncLatestReadOnlyReactiveProperty<T>(source, initialValue);
    }
  }
}