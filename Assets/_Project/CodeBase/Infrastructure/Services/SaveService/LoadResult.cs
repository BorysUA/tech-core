using _Project.CodeBase.Data.Progress;

namespace _Project.CodeBase.Infrastructure.Services
{
  public readonly struct LoadResult
  {
    public LoadStatus LoadStatus { get; }
    public GameStateData GameStateData { get; }

    public LoadResult(LoadStatus loadStatus, GameStateData gameStateData)
    {
      GameStateData = gameStateData;
      LoadStatus = loadStatus;
    }
  }
}