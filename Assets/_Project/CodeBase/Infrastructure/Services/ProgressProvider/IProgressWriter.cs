using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services.ProgressProvider
{
  public interface IProgressWriter
  {
    public IGameStateWriter GameStateModel { get; }
  }
}