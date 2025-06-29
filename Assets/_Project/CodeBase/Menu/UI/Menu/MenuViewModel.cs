using _Project.CodeBase.Menu.UI.DifficultySelection;
using _Project.CodeBase.Menu.UI.SaveSelection;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;

namespace _Project.CodeBase.Menu.UI.Menu
{
  public class MenuViewModel : BaseWindowViewModel
  {
    private readonly IWindowsService _windowsService;

    public MenuViewModel(IWindowsService windowsService)
    {
      _windowsService = windowsService;
    }

    public void SwitchToDifficultySelection()
    {
      _windowsService.OpenWindow<DifficultySelectionWindow, DifficultySelectionViewModel>();
    }

    public void SwitchToLoadSavedGames()
    {
      _windowsService.OpenWindow<SaveSelectionWindow, SaveSelectionViewModel>();
    }
  }
}