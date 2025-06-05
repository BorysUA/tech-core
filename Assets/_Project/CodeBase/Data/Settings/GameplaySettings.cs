using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Data.Settings
{
  public class GameplaySettings
  {
    public static GameplaySettings Default { get; } = new()
    {
      SaveSlot = SaveSlot.None,
      GameDifficulty = GameDifficulty.Medium
    };

    public SaveSlot SaveSlot;
    public GameDifficulty GameDifficulty;
  }
}