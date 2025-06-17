using System;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using R3;

namespace _Project.CodeBase.Services.DateTimeService
{
  public interface IDateTimeService : IServiceReadyAwaiter
  {
    ReadOnlyReactiveProperty<DateTime> ServerTime { get; }
    ReadOnlyReactiveProperty<DateTime> LocalTime { get; }
    public bool IsServerTimeAvailable { get; }
  }
}