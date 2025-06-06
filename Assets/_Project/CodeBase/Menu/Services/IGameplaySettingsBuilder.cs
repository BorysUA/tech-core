using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Menu.Services
{
  public interface IGameplaySettingsBuilder
  {
    public ReadOnlyReactiveProperty<GameDifficulty> GameDifficulty { get; }
    public ReadOnlyReactiveProperty<SaveSlot> SaveSlot { get; }
    public void SetGameDifficulty(GameDifficulty gameDifficulty);
    public void SetSaveSlot(SaveSlot saveSlot);
    public GameplaySettings Build();
    public void Reset();
    void SetSessionName(string sessionName);
  }
}