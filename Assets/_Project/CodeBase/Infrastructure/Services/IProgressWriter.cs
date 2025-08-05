using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public interface IProgressWriter
  {
    public IGameStateWriter GameStateModel { get; }
  }
}