using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.Window;
using _Project.CodeBase.UI.Common;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveBrowser
{
  public class SaveSelectionViewModel : BaseWindowViewModel, IAsyncInitializable
  {
    private readonly ISaveStorageService _saveStorageService;
    private readonly SignalBus _signalBus;
    private readonly ObservableList<SaveMetaData> _saveSlots = new();
    private readonly GameplaySettings _gameplaySettings;
    private readonly IWindowsService _windowsService;

    public IObservableCollection<SaveMetaData> SaveSlots => _saveSlots;

    public SaveSelectionViewModel(ISaveStorageService saveStorageService, GameplaySettings gameplaySettings,
      SignalBus signalBus, IWindowsService windowsService)
    {
      _saveStorageService = saveStorageService;
      _gameplaySettings = gameplaySettings;
      _signalBus = signalBus;
      _windowsService = windowsService;
    }

    public async UniTask InitializeAsync()
    {
      await LoadSavesMeta();
    }

    public void LoadSave(SaveMetaData saveMetaData)
    {
      _gameplaySettings.GameDifficulty = saveMetaData.Difficulty;
      _gameplaySettings.SaveSlot = saveMetaData.SaveSlot;
      _signalBus.Fire(new LoadGameplaySignal(_gameplaySettings));
    }

    public void DeleteSave(SaveMetaData saveMetaData)
    {
      _saveStorageService.ClearSlotManual(saveMetaData.SaveSlot);
      _saveSlots.Remove(saveMetaData);
    }

    private async UniTask LoadSavesMeta()
    {
      IEnumerable<SaveMetaData> savesMeta = await _saveStorageService.GetAllSavesMeta();

      foreach (SaveMetaData saveMetaData in savesMeta)
        _saveSlots.Add(saveMetaData);
    }

    public void BackToMenu() =>
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
  }
}