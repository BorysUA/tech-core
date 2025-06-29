using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Menu.Services;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using Zenject;

namespace _Project.CodeBase.Menu.UI.DifficultySelection
{
  public class DifficultySelectionViewModel : BaseWindowViewModel
  {
    private readonly SignalBus _signalBus;
    private readonly IWindowsService _windowsService;
    private readonly IGameplaySettingsBuilder _gameplaySettingsBuilder;

    public DifficultySelectionViewModel(SignalBus signalBus,
      IWindowsService windowsService, IGameplaySettingsBuilder gameplaySettingsBuilder)
    {
      _signalBus = signalBus;
      _windowsService = windowsService;
      _gameplaySettingsBuilder = gameplaySettingsBuilder;
    }

    public void ApplyDifficultySelection(GameDifficulty difficulty)
    {
      _gameplaySettingsBuilder.SetGameDifficulty(difficulty);
      GameplaySettings gameplaySettings = _gameplaySettingsBuilder.Build();
      _signalBus.Fire(new GameplaySceneLoadRequested(gameplaySettings));
    }

    public void BackToMenu()
    {
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
    }
  }
}