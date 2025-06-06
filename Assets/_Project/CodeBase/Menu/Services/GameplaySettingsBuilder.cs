using System;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Menu.Services
{
  public class GameplaySettingsBuilder : IGameplaySettingsBuilder, IDisposable
  {
    private readonly ReactiveProperty<GameDifficulty> _gameDifficulty = new(GameplaySettings.Default.GameDifficulty);
    private readonly ReactiveProperty<SaveSlot> _saveSlot = new(GameplaySettings.Default.SaveSlot);
    private readonly ReactiveProperty<string> _sessionName = new(GameplaySettings.Default.SessionName);

    public ReadOnlyReactiveProperty<GameDifficulty> GameDifficulty => _gameDifficulty;
    public ReadOnlyReactiveProperty<SaveSlot> SaveSlot => _saveSlot;
    public ReadOnlyReactiveProperty<string> SessionName => _sessionName;

    public void SetGameDifficulty(GameDifficulty gameDifficulty) =>
      _gameDifficulty.Value = gameDifficulty;

    public void SetSaveSlot(SaveSlot saveSlot) =>
      _saveSlot.Value = saveSlot;

    public void SetSessionName(string sessionName) =>
      _sessionName.Value = sessionName;

    public GameplaySettings Build()
    {
      return new GameplaySettings()
      {
        SaveSlot = _saveSlot.Value,
        GameDifficulty = _gameDifficulty.Value,
      };
    }

    public void Reset()
    {
      _saveSlot.Value = GameplaySettings.Default.SaveSlot;
      _gameDifficulty.Value = GameplaySettings.Default.GameDifficulty;
      _sessionName.Value = GameplaySettings.Default.SessionName;
    }

    public void Dispose()
    {
      _gameDifficulty.Dispose();
      _saveSlot.Dispose();
      _sessionName.Dispose();
    }
  }
}