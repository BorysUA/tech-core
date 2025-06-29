using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Menu.Services;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveSelection
{
  public class SaveSelectionViewModel : BaseWindowViewModel, IAsyncInitializable
  {
    private readonly ISaveStorageService _saveStorageService;
    private readonly SignalBus _signalBus;
    private readonly ObservableList<SaveMetaData> _saveSlots = new();
    private readonly IWindowsService _windowsService;
    private readonly IGameplaySettingsBuilder _gameplaySettingsBuilder;

    public IObservableCollection<SaveMetaData> SaveSlots => _saveSlots;

    public SaveSelectionViewModel(ISaveStorageService saveStorageService, SignalBus signalBus,
      IWindowsService windowsService, IGameplaySettingsBuilder gameplaySettingsBuilder)
    {
      _saveStorageService = saveStorageService;
      _signalBus = signalBus;
      _windowsService = windowsService;
      _gameplaySettingsBuilder = gameplaySettingsBuilder;
    }

    public async UniTask InitializeAsync()
    {
      await LoadSavesMeta();
    }

    public void LoadSave(SaveMetaData saveMetaData)
    {
      _gameplaySettingsBuilder.SetSaveSlot(saveMetaData.SaveSlot);
      _gameplaySettingsBuilder.SetGameDifficulty(saveMetaData.Difficulty);
      GameplaySettings gameplaySettings = _gameplaySettingsBuilder.Build();
      _signalBus.Fire(new GameplaySceneLoadRequested(gameplaySettings));
    }

    public void DeleteSave(SaveMetaData saveMetaData)
    {
      _saveStorageService.ClearSlotManual(saveMetaData.SaveSlot);
      _saveSlots.Remove(saveMetaData);
    }

    public void BackToMenu() =>
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();

    private async UniTask LoadSavesMeta()
    {
      IEnumerable<SaveMetaData> savesMeta = await _saveStorageService.GetAllSavesMeta();

      foreach (SaveMetaData saveMetaData in savesMeta)
        _saveSlots.Add(saveMetaData);
    }
  }
}