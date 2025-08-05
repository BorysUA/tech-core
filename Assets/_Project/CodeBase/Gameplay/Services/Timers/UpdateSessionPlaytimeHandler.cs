using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services;

namespace _Project.CodeBase.Gameplay.Services.Timers
{
  public class UpdateSessionPlaytimeHandler : ICommandHandler<UpdateSessionPlaytimeCommand, Unit>
  {
    private readonly IProgressWriter _progressWriter;

    public UpdateSessionPlaytimeHandler(IProgressWriter progressWriter)
    {
      _progressWriter = progressWriter;
    }

    public Unit Execute(in UpdateSessionPlaytimeCommand command)
    {
      ISessionInfoWriter sessionInfo = _progressWriter.GameStateModel.SessionInfoWriter;
      sessionInfo.TotalPlayTime.Value += command.SessionDeltaSeconds;
      return Unit.Default;
    }
  }
}