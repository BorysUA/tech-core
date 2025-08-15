using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.AssetsPipeline;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Menu.UI.SaveSelection;
using _Project.CodeBase.UI.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Menu.UI.Factories
{
  public class MenuUiFactory : IMenuUiFactory
  {
    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;
    private readonly AddressMap _addressMap;

    private WindowsCanvas _windowsCanvas;

    public MenuUiFactory(IInstantiator instantiator, IAssetProvider assetProvider,
      AddressMap addressMap, WindowsCanvas windowsCanvas)
    {
      _assetProvider = assetProvider;
      _addressMap = addressMap;
      _instantiator = instantiator;
      _windowsCanvas = windowsCanvas;
    }

    public async UniTask<SaveSlotItem> CreateSaveSlot(SaveMetaData data, Transform saveSlotsContainer)
    {
      GameObject saveSpotPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.SaveSlotButton);
      SaveSlotItem saveSlotItem =
        _instantiator.InstantiatePrefabForComponent<SaveSlotItem>(saveSpotPrefab, saveSlotsContainer);

      saveSlotItem.Setup(data.SaveSlot, data.DisplayName, data.InGameTime, data.LastModifiedUtc);

      return saveSlotItem;
    }
  }
}