using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services.ProgressProvider
{
  public interface IProgressSaver
  {
    public IGameStateSaver GameStateModel { get; }
  }
}