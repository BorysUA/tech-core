using System;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Menu.States;
using _Project.CodeBase.Menu.UI.SaveSelection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.CodeBase.Menu.UI.Factories
{
  public class MenuUiFactory : IMenuUiFactory, IMenuInitAsync, IDisposable
  {
    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;

    private GameObject _saveSpotPrefab;
    private readonly ObjectPool<SaveSlotItem> _saveSlotPool = new();

    public MenuUiFactory(IInstantiator instantiator, IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
      _instantiator = instantiator;
    }

    public async UniTask InitializeAsync() =>
      _saveSpotPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.SaveSlotButton);

    public SaveSlotItem CreateSaveSlot(SaveMetaData data, Transform saveSlotsContainer)
    {
      if (!_saveSlotPool.TryGet(out SaveSlotItem saveSlotItem))
      {
        saveSlotItem = _instantiator.InstantiatePrefabForComponent<SaveSlotItem>(_saveSpotPrefab, saveSlotsContainer);
        _saveSlotPool.Add(saveSlotItem);
      }

      saveSlotItem.Setup(data.SaveSlot, data.DisplayName, data.InGameTime, data.LastModifiedUtc);

      return saveSlotItem;
    }

    public void Dispose()
    {
      _assetProvider.ReleaseAsset(AssetAddress.SaveSlotButton);
    }
  }
}