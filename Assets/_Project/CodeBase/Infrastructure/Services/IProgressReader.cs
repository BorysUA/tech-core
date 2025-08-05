using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public interface IProgressReader
  {
    public IGameStateReader GameStateModel { get; }
  }
}