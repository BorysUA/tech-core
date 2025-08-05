using _Project.CodeBase.Data.Progress;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface IGameStateSaver
  {
    public GameStateData GameStateData { get; }
  }
}