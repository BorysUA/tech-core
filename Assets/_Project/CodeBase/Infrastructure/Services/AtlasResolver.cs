using System;
using System.Collections.Generic;
using _Project.CodeBase.Gameplay.States;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine.U2D;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class AtlasResolver : IBootstrapInitAsync, IDisposable
  {
    private readonly IAssetProvider _assetProvider;
    private readonly Dictionary<string, SpriteAtlas> _spriteAtlases = new();

    private bool _isInitialized;

    public AtlasResolver(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;

      SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    public async UniTask InitializeAsync()
    {
      await LoadSpriteAtlas(AssetAddress.CoreAtlas);
      _isInitialized = true;
    }

    private async UniTask LoadSpriteAtlas(string address)
    {
      SpriteAtlas atlas = await _assetProvider.LoadAssetAsync<SpriteAtlas>(address);
      _spriteAtlases.Add(atlas.name, atlas);
    }

    private async void OnAtlasRequested(string atlasName, Action<SpriteAtlas> callback)
    {
      await UniTask.WaitUntil(() => _isInitialized);

      if (_spriteAtlases.TryGetValue(atlasName, out SpriteAtlas spriteAtlas))
        callback?.Invoke(spriteAtlas);
      else
        throw new NullReferenceException("Atlas not found");
    }

    public void Dispose()
    {
      SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
    }
  }
}