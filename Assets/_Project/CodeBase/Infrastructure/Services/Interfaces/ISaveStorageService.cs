using System;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using Cysharp.Threading.Tasks;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface ISaveStorageService : IDisposable
  {
    public UniTask SaveGameAsync(GameStateData data, SaveSlot saveSlot, CancellationToken token = default);
    public UniTask<LoadResult> LoadGameAsync(SaveSlot saveSlot, CancellationToken token = default);
    public UniTask<IEnumerable<SaveMetaData>> GetAllSavesMeta();
    public void ClearSlotManual(SaveSlot saveSlot);
  }
}