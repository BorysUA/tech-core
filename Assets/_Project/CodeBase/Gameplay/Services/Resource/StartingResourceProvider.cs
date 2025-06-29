using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public class StartingResourceProvider : IStartingResourcesProvider
  {
    private readonly Dictionary<GameDifficulty, GameResourceData[]> _presets = new()
    {
      [GameDifficulty.Easy] = new[]
      {
        new GameResourceData(ResourceKind.Coin, 300, 1000),
        new GameResourceData(ResourceKind.Metal, 300, 1000),
        new GameResourceData(ResourceKind.Energy, 300, 1000),
        new GameResourceData(ResourceKind.Population, 300, 1000),
      },
      [GameDifficulty.Medium] = new[]
      {
        new GameResourceData(ResourceKind.Coin, 200, 700),
        new GameResourceData(ResourceKind.Metal, 200, 700),
        new GameResourceData(ResourceKind.Energy, 200, 700),
        new GameResourceData(ResourceKind.Population, 200, 700),
      },
      [GameDifficulty.Hard] = new[]
      {
        new GameResourceData(ResourceKind.Coin, 100, 500),
        new GameResourceData(ResourceKind.Metal, 100, 500),
        new GameResourceData(ResourceKind.Energy, 100, 500),
        new GameResourceData(ResourceKind.Population, 100, 500),
      }
    };

    public GameResourceData[] GetInitialResources(GameDifficulty difficulty)
    {
      return _presets[difficulty];
    }
  }
}