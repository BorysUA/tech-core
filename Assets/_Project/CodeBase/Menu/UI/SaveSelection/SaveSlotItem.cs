using System;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Menu.UI.SaveSelection
{
  public class SaveSlotItem : MonoBehaviour, IPoolItem<PoolUnit>
  {
    [SerializeField] private Button _loadSaveButton;
    [SerializeField] private Button _deleteSaveButton;

    [SerializeField] private TextMeshProUGUI _saveSlotText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private TextMeshProUGUI _inGameTimeText;
    [SerializeField] private TextMeshProUGUI _lastModifiedUtcText;

    private readonly Subject<Unit> _deactivated = new();
    private SaveSlot _saveSlot;
    private DisposableBag _lifecycle;

    public Observable<Unit> Deactivated => _deactivated;

    public void Setup(SaveSlot saveSlot, string displayName, TimeSpan inGameTime, DateTime lastModifiedTime)
    {
      _saveSlot = saveSlot;

      _saveSlotText.SetText($"Save slot: {saveSlot.ToStringKey()}");
      _displayNameText.SetText($"Location: {displayName}");
      _inGameTimeText.SetText("Playtime: {0:00}:{1:00}:{2:00}", (int)inGameTime.TotalHours, inGameTime.Minutes,
        inGameTime.Seconds);
      _lastModifiedUtcText.SetText($"Last modified: {lastModifiedTime:yyyy-MM-dd HH:mm}");
    }

    public void BindActive(Action<SaveSlot> onLoad, Action<SaveSlot> onDelete)
    {
      _loadSaveButton
        .OnClickAsObservable()
        .Subscribe(_ => onLoad.Invoke(_saveSlot))
        .AddTo(ref _lifecycle);

      _deleteSaveButton
        .OnClickAsObservable()
        .Subscribe(_ => onDelete.Invoke(_saveSlot))
        .AddTo(ref _lifecycle);
    }

    public void Activate(PoolUnit param)
    {
      gameObject.SetActive(true);
    }

    public void Deactivate()
    {
      gameObject.SetActive(false);
      _lifecycle.Clear();
      _deactivated.OnNext(Unit.Default);
    }
  }
}