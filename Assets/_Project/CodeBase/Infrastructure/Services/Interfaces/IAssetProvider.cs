using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IAssetProvider
  {
    void CleanUp();
    UniTask InitializeAsync();
    UniTask PreloadAssetsAsync(string label);

    UniTask<T> LoadAssetAsync<T>(string address, CancellationToken token = default, int retryCount = 1)
      where T : class;

    UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, CancellationToken token = default, int retryCount = 1)
      where T : class;
  }
}