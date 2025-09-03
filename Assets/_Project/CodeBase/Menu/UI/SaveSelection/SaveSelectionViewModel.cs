using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Data.Settings;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.Services;
using _Project.CodeBase.Menu.Signals;
using _Project.CodeBase.Menu.UI.Menu;
using _Project.CodeBase.UI.Core;
using _Project.CodeBase.UI.Services;
using ObservableCollections;
using R3;
using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveSelection
{
  public class SaveSelectionViewModel : BaseWindowViewModel
  {
    private readonly ISaveStorageService _saveStorageService;
    private readonly SignalBus _signalBus;
    private readonly IWindowsService _windowsService;
    private readonly IGameplaySettingsBuilder _gameplaySettingsBuilder;

    private readonly Subject<Unit> _initialized = new();
    private readonly ObservableList<SaveMetaData> _saveSlots = new();

    public IReadOnlyObservableList<SaveMetaData> SaveSlots => _saveSlots;

    public Observable<Unit> Initialized => _initialized;

    public SaveSelectionViewModel(ISaveStorageService saveStorageService, SignalBus signalBus,
      IWindowsService windowsService, IGameplaySettingsBuilder gameplaySettingsBuilder)
    {
      _saveStorageService = saveStorageService;
      _signalBus = signalBus;
      _windowsService = windowsService;
      _gameplaySettingsBuilder = gameplaySettingsBuilder;
    }

    public override void Initialize()
    {
      base.Initialize();

      IEnumerable<SaveMetaData> savesMeta = _saveStorageService
        .GetSavedGamesMeta()
        .OrderByDescending(saveMeta => saveMeta.LastModifiedUtc);

      _saveSlots.AddRange(savesMeta);

      _initialized.OnNext(Unit.Default);
    }

    public override void Close()
    {
      _saveSlots.Clear();
      base.Close();
    }

    public void LoadSave(SaveSlot saveSlot)
    {
      if (!_saveStorageService.TryGetSaveMeta(saveSlot, out SaveMetaData saveMeta))
        return;

      _gameplaySettingsBuilder.SetSaveSlot(saveSlot);
      _gameplaySettingsBuilder.SetGameDifficulty(saveMeta.Difficulty);
      GameplaySettings gameplaySettings = _gameplaySettingsBuilder.Build();
      _signalBus.Fire(new GameplaySceneLoadRequested(gameplaySettings));
    }

    public void DeleteSave(SaveSlot saveSlot)
    {
      if (!_saveStorageService.TryGetSaveMeta(saveSlot, out SaveMetaData saveMeta))
        return;

      _saveStorageService.ClearSlotManual(saveSlot);
      _saveSlots.Remove(saveMeta);
    }

    public void BackToMenu() =>
      _windowsService.OpenWindow<MenuWindow, MenuViewModel>();
  }
}