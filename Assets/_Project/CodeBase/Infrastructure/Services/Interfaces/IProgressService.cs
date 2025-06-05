using _Project.CodeBase.Gameplay.Data;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IProgressService
  {
    public GameStateProxy GameStateProxy { get; }
  }
}