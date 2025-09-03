using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IAssetProvider : IServiceReadyAwaiter
  {
    void CleanUp();

    UniTask<T> LoadAssetAsync<T>(string address, CancellationToken token = default, int retryCount = 1)
      where T : class;

    UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, CancellationToken token = default, int retryCount = 1)
      where T : class;

    UniTask<T> LoadAssetAsyncFromResources<T>(string path) where T : Object;

    UniTask<IList<T>> LoadAssetsAsync<T>(string address, CancellationToken token = default, int retryCount = 1)
      where T : class;

    void ReleaseAsset(string address);
  }
}