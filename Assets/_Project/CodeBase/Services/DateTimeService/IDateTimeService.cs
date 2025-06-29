using System;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using R3;

namespace _Project.CodeBase.Services.DateTimeService
{
  public interface IDateTimeService : IServiceReadyAwaiter
  {
    public bool IsServerTimeAvailable { get; }
    ReadOnlyReactiveProperty<DateTime> CurrentTime { get; }
  }
}