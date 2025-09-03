using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.UI.Factories;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveSelection
{
  public class SaveSelectionWindow : BaseWindow<SaveSelectionViewModel>
  {
    [SerializeField] private Transform _saveSlotsContainer;
    [SerializeField] private Button _backToMenuButton;

    private readonly Dictionary<SaveSlot, SaveSlotItem> _saveSlotItems = new();
    private IMenuUiFactory _uiFactory;

    [Inject]
    public void Construct(IMenuUiFactory uiFactory)
    {
      _uiFactory = uiFactory;
    }

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      ViewModel.Initialized
        .Subscribe(_ => FillSlots())
        .AddTo(this);

      _backToMenuButton
        .OnClickAsObservable()
        .Subscribe(_ => ViewModel.BackToMenu())
        .AddTo(this);

      ViewModel.SaveSlots
        .ObserveRemove()
        .Subscribe(removeEvent =>
        {
          if (_saveSlotItems.Remove(removeEvent.Value.SaveSlot, out SaveSlotItem saveSlotItem))
            saveSlotItem.Deactivate();
        })
        .AddTo(this);

      ViewModel.SaveSlots
        .ObserveClear()
        .Subscribe(_ =>
        {
          foreach (var saveSlot in _saveSlotItems)
            saveSlot.Value.Deactivate();

          _saveSlotItems.Clear();
        })
        .AddTo(this);
    }

    private void FillSlots()
    {
      foreach (SaveMetaData saveSlot in ViewModel.SaveSlots)
        CreateSaveSlot(saveSlot);
    }

    private void CreateSaveSlot(SaveMetaData saveSlot)
    {
      SaveSlotItem selectSaveItem = _uiFactory.CreateSaveSlot(saveSlot, _saveSlotsContainer);

      selectSaveItem.BindActive(LoadSave, DeleteSave);
      _saveSlotItems.Add(saveSlot.SaveSlot, selectSaveItem);
    }

    private void LoadSave(SaveSlot saveSlot) =>
      ViewModel.LoadSave(saveSlot);

    private void DeleteSave(SaveSlot saveSlot) =>
      ViewModel.DeleteSave(saveSlot);
  }
}