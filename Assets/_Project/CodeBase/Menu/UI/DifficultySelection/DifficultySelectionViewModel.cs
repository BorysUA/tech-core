using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.Window;
using _Project.CodeBase.UI.Common;
using _Project.CodeBase.UI.Services;
using Zenject;

namespace _Project.CodeBase.Menu.UI.DifficultySelection
{
  public class DifficultySelectionViewModel : BaseWindowViewModel
  {
    private readonly GameplaySettings _gameplaySettings;
    private readonly SignalBus _signalBus;
    private readonly IWindowsService _windowsService;

    public DifficultySelectionViewModel(GameplaySettings gameplaySettings, SignalBus signalBus,
      IWindowsService windowsService)
    {
      _gameplaySettings = gameplaySettings;
      _signalBus = signalBus;
      _windowsService = windowsService;
    }

    public void ApplyDifficultySelection(GameDifficulty difficulty)
    {
      _gameplaySettings.GameDifficulty = difficulty;
      _gameplaySettings.SaveSlot = SaveSlot.None;
      _signalBus.Fire(new LoadGameplaySignal(_gameplaySettings));
    }

    public void BackToMenu()
    {
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
    }
  }
}