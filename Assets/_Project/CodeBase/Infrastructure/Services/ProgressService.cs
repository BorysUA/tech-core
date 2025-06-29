using _Project.CodeBase.Gameplay.DataProxy;
using _Project.CodeBase.Infrastructure.Services.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class ProgressService : IProgressService
  {
    public GameStateProxy GameStateProxy { get; } = new();
  }
}