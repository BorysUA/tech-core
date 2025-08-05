using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public interface IProgressSaver
  {
    public IGameStateSaver GameStateModel { get; }
  }
}