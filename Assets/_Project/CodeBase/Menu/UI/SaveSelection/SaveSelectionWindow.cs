using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Menu.UI.Factories;
using _Project.CodeBase.UI.Common;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Menu.UI.SaveBrowser
{
  public class SaveSelectionWindow : BaseWindow<SaveSelectionViewModel>
  {
    [SerializeField] private Transform _saveSlotsContainer;
    [SerializeField] private Button _backToMenuButton;

    private IMenuUiFactory _uiFactory;

    [Inject]
    public void Construct(IMenuUiFactory uiFactory)
    {
      _uiFactory = uiFactory;
    }

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      foreach (SaveMetaData saveSlots in ViewModel.SaveSlots)
        CreateSaveSlot(saveSlots);

      ViewModel.SaveSlots
        .ObserveAdd()
        .Subscribe(addEvent => CreateSaveSlot(addEvent.Value))
        .AddTo(this);

      _backToMenuButton
        .OnClickAsObservable()
        .Subscribe(_ => ViewModel.BackToMenu())
        .AddTo(this);
    }

    private async void CreateSaveSlot(SaveMetaData saveSlot)
    {
      SaveSlotItem selectSaveItem =
        await _uiFactory.CreateSaveSlot(saveSlot, _saveSlotsContainer);

      selectSaveItem.OnLoadSaveClick
        .Subscribe(_ => ViewModel.LoadSave(saveSlot))
        .AddTo(this);

      selectSaveItem.OnDeleteSaveClick
        .Subscribe(_ =>
        {
          selectSaveItem.Hide();
          ViewModel.DeleteSave(saveSlot);
        })
        .AddTo(this);
    }
  }
}