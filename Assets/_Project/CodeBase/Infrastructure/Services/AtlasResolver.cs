using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using UnityEngine.U2D;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class AtlasResolver : IBootstrapInit, IDisposable
  {
    private static readonly Dictionary<string, string> NameToAddressMap = new()
    {
      { "Core", AssetAddress.CoreAtlas }
    };

    private readonly IAssetProvider _assetProvider;
    private readonly ILogService _logService;
    private readonly Dictionary<string, SpriteAtlas> _spriteAtlases = new();

    public AtlasResolver(IAssetProvider assetProvider, ILogService logService)
    {
      _assetProvider = assetProvider;
      _logService = logService;
    }

    public void Initialize()
    {
      SpriteAtlasManager.atlasRequested += OnAtlasRequested;
    }

    private void OnAtlasRequested(string atlasName, Action<SpriteAtlas> callback)
    {
      UniTask.Void(async () =>
      {
        try
        {
          await _assetProvider.WhenReady;

          if (_spriteAtlases.TryGetValue(atlasName, out SpriteAtlas spriteAtlas))
          {
            callback?.Invoke(spriteAtlas);
            return;
          }

          if (!NameToAddressMap.TryGetValue(atlasName, out string address))
          {
            _logService.LogError(GetType(), $"Unknown atlas requested: {atlasName}");
            return;
          }

          SpriteAtlas atlas = await _assetProvider.LoadAssetAsync<SpriteAtlas>(address);
          _spriteAtlases[atlasName] = atlas;

          callback?.Invoke(atlas);
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), $"Failed to load atlas {atlasName}", exception);
        }
      });
    }

    public void Dispose() =>
      SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
  }
}