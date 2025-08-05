using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Services.Timers
{
  public readonly struct UpdateSessionPlaytimeCommand : ICommand<Unit>
  {
    public float SessionDeltaSeconds { get; }

    public UpdateSessionPlaytimeCommand(float sessionDeltaSeconds)
    {
      SessionDeltaSeconds = sessionDeltaSeconds;
    }
  }
}