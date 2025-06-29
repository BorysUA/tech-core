using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Gameplay.Services.Resource
{
  public interface IStartingResourcesProvider
  {
    GameResourceData[] GetInitialResources(GameDifficulty difficulty);
  }
}