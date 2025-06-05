using System;
using _Project.CodeBase.Infrastructure.Services;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.CodeBase.Menu.UI.SaveBrowser
{
  public class SaveSlotItem : MonoBehaviour
  {
    [SerializeField] private Button _loadSaveButton;
    [SerializeField] private Button _deleteSaveButton;

    [SerializeField] private TextMeshProUGUI _saveSlotText;
    [SerializeField] private TextMeshProUGUI _displayNameText;
    [SerializeField] private TextMeshProUGUI _inGameTimeText;
    [SerializeField] private TextMeshProUGUI _lastModifiedUtcText;

    public Observable<Unit> OnLoadSaveClick =>
      _loadSaveButton.OnClickAsObservable();

    public Observable<Unit> OnDeleteSaveClick =>
      _deleteSaveButton.OnClickAsObservable();

    public void Setup(SaveSlot saveSlot, string displayName, TimeSpan inGameTime, DateTime lastModifiedTime)
    {
      _saveSlotText.SetText($"Save slot: {saveSlot}");
      _displayNameText.SetText($"Location: {displayName}");
      _inGameTimeText.SetText($"Playtime: {FormatTime(inGameTime)}");
      _lastModifiedUtcText.SetText($"Last modified: {lastModifiedTime}");
    }

    public void Hide() =>
      Destroy(gameObject);

    private string FormatTime(TimeSpan time)
    {
      return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
    }
  }
}