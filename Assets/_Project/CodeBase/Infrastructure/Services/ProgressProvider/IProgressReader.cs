using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services.ProgressProvider
{
  public interface IProgressReader
  {
    public IGameStateReader GameStateModel { get; }
  }
}