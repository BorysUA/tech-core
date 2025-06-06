using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using R3;

namespace _Project.CodeBase.Infrastructure.Services.SaveService
{
  public class GameSaveService : IGameSaveService, IOnLoadInitializable, IDisposable
  {
    private const float SaveInterval = 10f;

    private readonly SaveSlot[] _manualSaveSlotsOrder =
    {
      SaveSlot.Manual1, SaveSlot.Manual2, SaveSlot.Manual3, SaveSlot.Manual4, SaveSlot.Manual5,
      SaveSlot.Manual6, SaveSlot.Manual7, SaveSlot.Manual8, SaveSlot.Manual9, SaveSlot.Manual10
    };

    private readonly ISaveStorageService _saveStorageService;
    private readonly IProgressService _progressService;
    private readonly ILogService _logService;
    private readonly CompositeDisposable _disposable = new();

    private bool _isSaving;
    private float _lastSaveTime;

    public GameSaveService(ISaveStorageService saveStorageService, IProgressService progressService,
      ILogService logService)
    {
      _saveStorageService = saveStorageService;
      _progressService = progressService;
      _logService = logService;
    }

    public void Initialize()
    {
      InitializeAutosave();
    }

    private void InitializeAutosave()
    {
      _progressService.GameStateProxy.SessionInfo.SessionPlayTime
        .Subscribe(SaveAutoAsync)
        .AddTo(_disposable);
    }

    public async UniTask SaveManualAsync(CancellationToken token = default)
    {
      if (_isSaving)
        return;

      _isSaving = true;
      try
      {
        SaveSlot slot = await PickManualSlot();
        await _saveStorageService.SaveGameAsync(_progressService.GameStateProxy.GameStateData, slot, token);
      }
      catch (OperationCanceledException)
      {
        _logService.LogInfo(GetType(), "Manual save was cancelled.");
      }
      catch (Exception exception)
      {
        _logService.LogError(GetType(), $"Manual save to slot failed", exception);
        throw;
      }
      finally
      {
        _isSaving = false;
      }
    }

    public void Dispose()
    {
      _disposable?.Dispose();
    }

    private async void SaveAutoAsync(float sessionTime)
    {
      if (_isSaving)
        return;

      if (sessionTime - _lastSaveTime <= SaveInterval)
        return;

      _isSaving = true;
      try
      {
        await _saveStorageService.SaveGameAsync(_progressService.GameStateProxy.GameStateData, SaveSlot.Auto);
        _lastSaveTime = sessionTime;
      }
      catch (Exception ex)
      {
        _logService.LogError(GetType(), "Auto-save failed", ex);
      }
      finally
      {
        _isSaving = false;
      }
    }

    private async UniTask<SaveSlot> PickManualSlot()
    {
      List<SaveMetaData> manualSaves = (await _saveStorageService.GetAllSavesMeta())
        .Where(meta => IsManual(meta.SaveSlot))
        .ToList();

      HashSet<SaveSlot> occupiedSlots = new HashSet<SaveSlot>(manualSaves.Select(meta => meta.SaveSlot));

      foreach (SaveSlot slot in _manualSaveSlotsOrder)
      {
        if (!occupiedSlots.Contains(slot))
          return slot;
      }

      SaveMetaData oldest = manualSaves
        .OrderBy(meta => meta.LastModifiedUtc)
        .First();

      return oldest.SaveSlot;
    }

    private bool IsManual(SaveSlot slot) =>
      slot is >= SaveSlot.Manual1 and <= SaveSlot.Manual10;
  }
}