using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Models.Persistent;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class PersistentProgressService : IProgressReader, IProgressWriter, IProgressSaver
  {
    private GameStateModel _gameStateModel;
    
    IGameStateReader IProgressReader.GameStateModel => _gameStateModel;
    IGameStateWriter IProgressWriter.GameStateModel => _gameStateModel;
    IGameStateSaver IProgressSaver.GameStateModel => _gameStateModel;

    public void Initialize(GameStateData gameStateData)
    {
      _gameStateModel = new GameStateModel(gameStateData);
    }
  }
}