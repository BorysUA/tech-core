using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Data.Settings
{
  public readonly struct GameplaySettings
  {
    public SaveSlot SaveSlot { get; init; }
    public string SessionName { get; init; }
    public GameDifficulty GameDifficulty { get; init; }
 
    public static GameplaySettings Default { get; } = new()
    {
      SaveSlot = SaveSlot.None,
      GameDifficulty = GameDifficulty.Medium,
      SessionName = "Mars colony"
    };
  }
}