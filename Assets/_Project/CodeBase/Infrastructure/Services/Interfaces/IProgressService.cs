using _Project.CodeBase.Gameplay.DataProxy;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IProgressService
  {
    public GameStateProxy GameStateProxy { get; }
  }
}