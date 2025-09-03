using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Infrastructure.StateMachine;
using _Project.CodeBase.Menu.States;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.Services.SaveService
{
  public class JsonSaveStorageService : ISaveStorageService, IMenuInitAsync
  {
    private const string JsonExtension = ".json";
    private const string BakExtension = ".bak";
    private const string TmpExtension = ".tmp";
    private const string MetaExtension = ".meta";
    private const int GateTimeoutMs = 30000;

    private readonly SemaphoreSlim _writeGate = new(1, 1);
    private readonly SemaphoreSlim _readGate = new(1, 1);
    private readonly string _saveRootPath = Application.persistentDataPath;
    private readonly string _applicationVersion = Application.version;
    private readonly ILogService _logService;
    private readonly JsonSerializer _writeSerializer;
    private readonly JsonSerializer _readSerializer;

    private readonly Dictionary<SaveSlot, WeakReference<GameStateData>> _gameStateCache = new();
    private readonly Dictionary<SaveSlot, SaveMetaData> _savesMetaCache = new();

    public JsonSaveStorageService(ILogService logService)
    {
      _logService = logService;

      var settings = new JsonSerializerSettings
      {
        Formatting = Formatting.None,
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        SerializationBinder = new SafeSerializationBinder(BuildAllowedTypes(), BuildTrustedAssemblies()),
        Converters = new List<JsonConverter>
        {
          new TypeKeyDictionaryConverter<BuildingData>(_logService),
          new TypeKeyDictionaryConverter<GameResourceData>(_logService),
          new TypeKeyDictionaryConverter<ResourceDropData>(_logService),
          new TypeKeyDictionaryConverter<IModuleData>(_logService),
        }
      };

      _writeSerializer = JsonSerializer.Create(settings);
      _readSerializer = JsonSerializer.Create(settings);

      Directory.CreateDirectory(_saveRootPath);
    }

    public async UniTask InitializeAsync()
    {
      await PreloadGameSavesMetaAsync();
    }

    public IEnumerable<SaveMetaData> GetSavedGamesMeta() =>
      _savesMetaCache.Values;

    public bool TryGetSaveMeta(SaveSlot saveSlot, out SaveMetaData saveMeta) =>
      _savesMetaCache.TryGetValue(saveSlot, out saveMeta);

    public async UniTask SaveGameAsync(GameStateData data, SaveSlot saveSlot, CancellationToken token = default)
    {
      _gameStateCache.Remove(saveSlot);
      string baseName = saveSlot.ToStringKey();

      string saveFilePath = Path.Combine(_saveRootPath, baseName + JsonExtension);
      string backupFilePath = Path.Combine(_saveRootPath, baseName + BakExtension);
      string tempFilePath = Path.Combine(_saveRootPath, baseName + TmpExtension);
      string metaFilePath = Path.Combine(_saveRootPath, baseName + MetaExtension);

      string tempBackupFilePath = backupFilePath + TmpExtension;
      string tempMetaFilePath = metaFilePath + TmpExtension;

      bool entered = false;

      try
      {
        entered = await _writeGate.WaitAsync(GateTimeoutMs, cancellationToken: token);

        if (!entered)
          throw new TimeoutException("Write gate timeout");

        try
        {
          await SaveProgress(data, tempFilePath, backupFilePath, tempBackupFilePath, saveFilePath, token);
        }
        catch (IOException ioException)
        {
          _logService.LogError(GetType(), "Unexpected error while reading/writing file", ioException);
          RestoreFromBackup(saveFilePath, backupFilePath, tempBackupFilePath);
          throw;
        }

        await GenerateMeta(data, saveSlot, tempMetaFilePath, metaFilePath, token);
      }
      catch (OperationCanceledException)
      {
        throw;
      }
      catch (Exception exception)
      {
        _logService.LogError(GetType(), "Failed to save player progress", exception);
        throw;
      }
      finally
      {
        if (entered)
          _writeGate.Release();

        try
        {
          if (File.Exists(tempFilePath))
            File.Delete(tempFilePath);
        }
        catch (Exception)
        {
          _logService.LogWarning(GetType(), $"Failed to delete temp file by path {tempFilePath}");
        }
      }
    }

    public async UniTask<LoadResult> LoadGameAsync(SaveSlot saveSlot, CancellationToken token = default)
    {
      if (_gameStateCache.TryGetValue(saveSlot, out WeakReference<GameStateData> weakReference))
        if (weakReference.TryGetTarget(out GameStateData gameStateData))
          return new LoadResult(LoadStatus.Success, gameStateData);

      string baseName = saveSlot.ToStringKey();

      string saveFilePath = Path.Combine(_saveRootPath, baseName + JsonExtension);

      bool entered = false;

      try
      {
        entered = await _readGate.WaitAsync(GateTimeoutMs, cancellationToken: token);

        if (!entered)
          throw new TimeoutException("Read gate timeout");

        if (File.Exists(saveFilePath))
        {
          LoadResult result = await UniTask.RunOnThreadPool(() =>
          {
            using FileStream fileStream = new FileStream(
              saveFilePath, FileMode.Open, FileAccess.Read,
              FileShare.Read, 32 * 1024, FileOptions.SequentialScan);

            using StreamReader streamReader = new StreamReader(
              fileStream, new UTF8Encoding(false), true, 32 * 1024);

            using JsonReader reader = new JsonTextReader(streamReader);

            token.ThrowIfCancellationRequested();

            GameStateData gameStateData = _readSerializer.Deserialize<GameStateData>(reader);
            return new LoadResult(LoadStatus.Success, gameStateData);
          }, cancellationToken: token).ContinueOnMainThread();

          _gameStateCache[saveSlot] = new WeakReference<GameStateData>(result.GameStateData);
          return result;
        }

        return new LoadResult(LoadStatus.Failed, null);
      }
      catch (OperationCanceledException)
      {
        throw;
      }
      catch (Exception exception)
      {
        _logService.LogError(GetType(), "Failed to load player progress", exception);
        throw;
      }
      finally
      {
        if (entered)
          _readGate.Release();
      }
    }

    public void ClearSlotManual(SaveSlot saveSlot)
    {
      string baseName = saveSlot.ToStringKey();

      foreach (string ext in new[] { JsonExtension, BakExtension, MetaExtension })
      {
        string path = Path.Combine(_saveRootPath, baseName + ext);
        try
        {
          File.Delete(path);
          _savesMetaCache.Remove(saveSlot);
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), $"Failed to delete player progress in save slot [{saveSlot}]", exception);
        }
      }
    }

    public void Dispose()
    {
      _writeGate?.Dispose();
      _readGate?.Dispose();
    }

    private async UniTask PreloadGameSavesMetaAsync(CancellationToken token = default)
    {
      _savesMetaCache.Clear();

      bool entered = false;
      try
      {
        entered = await _readGate.WaitAsync(GateTimeoutMs, cancellationToken: token);

        if (!entered)
          throw new TimeoutException("Read gate timeout");

        await UniTask.RunOnThreadPool(() =>
        {
          foreach (string filePath in Directory.EnumerateFiles(_saveRootPath, "*.meta"))
          {
            using FileStream fileStream = new FileStream(
              filePath, FileMode.Open, FileAccess.Read,
              FileShare.Read, 4 * 1024, FileOptions.SequentialScan);

            using StreamReader streamReader = new StreamReader(
              fileStream, new UTF8Encoding(false), true, 4 * 1024);

            using JsonReader reader = new JsonTextReader(streamReader);

            SaveMetaData meta = _readSerializer.Deserialize<SaveMetaData>(reader);
            _savesMetaCache.Add(meta.SaveSlot, meta);
          }
        }, cancellationToken: token).ContinueOnMainThread();
      }
      finally
      {
        if (entered)
          _readGate.Release();
      }
    }

    private async UniTask SaveProgress(GameStateData data, string tempFilePath, string backupFilePath,
      string tempBackupFilePath, string saveFilePath, CancellationToken token)
    {
      await UniTask.RunOnThreadPool(() =>
        {
          {
            using FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write,
              FileShare.None, 32 * 1024, false);
            using StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false), 32 * 1024, false);
            using JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);

            _writeSerializer.Serialize(jsonWriter, data, typeof(GameStateData));
          }

          ReplaceFile(backupFilePath, tempBackupFilePath);
          ReplaceFile(saveFilePath, backupFilePath);
          ReplaceFile(tempFilePath, saveFilePath);
        }, cancellationToken: token, configureAwait: false)
        .ContinueOnMainThread();
    }

    private async UniTask GenerateMeta(GameStateData data, SaveSlot saveSlot, string tempMetaFilePath,
      string metaFilePath, CancellationToken token)
    {
      SaveMetaData saveMetaData = new SaveMetaData
      {
        SaveSlot = saveSlot,
        Difficulty = data.SessionInfo.Difficulty,
        DisplayName = data.SessionInfo.ColonyName,
        InGameTime = TimeSpan.FromSeconds(data.SessionInfo.Playtime),
        LastModifiedUtc = DateTime.UtcNow,
        BuildVersion = _applicationVersion
      };

      await UniTask.RunOnThreadPool(() =>
        {
          {
            using FileStream fileStream = new FileStream(tempMetaFilePath, FileMode.Create, FileAccess.Write,
              FileShare.None, 4 * 1024, false);
            using StreamWriter streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false), 4 * 1024, false);
            using JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);

            _writeSerializer.Serialize(jsonWriter, saveMetaData, typeof(SaveMetaData));
          }

          ReplaceFile(tempMetaFilePath, metaFilePath);
        }, cancellationToken: token, configureAwait: false)
        .ContinueOnMainThread();

      _savesMetaCache[saveSlot] = saveMetaData;
      token.ThrowIfCancellationRequested();
    }

    private void RestoreFromBackup(string saveFilePath, string backupFilePath, string tempBackupFilePath)
    {
      if (!File.Exists(backupFilePath) && File.Exists(tempBackupFilePath))
      {
        try
        {
          File.Move(tempBackupFilePath, backupFilePath);
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), "Failed to rollback saveBackup from tempBackup", exception);
        }
      }

      if (!File.Exists(saveFilePath) && File.Exists(backupFilePath))
      {
        try
        {
          File.Copy(backupFilePath, saveFilePath);
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), "Failed to rollback saveFile from backup", exception);
        }
      }
    }

    private void ReplaceFile(string source, string destination)
    {
      if (!File.Exists(source))
        return;

      File.Delete(destination);

      File.Move(source, destination);
    }

    private IEnumerable<Type> BuildAllowedTypes()
    {
      List<Type> types = new List<Type>();

      types.AddRange(FindConcreteTypesImplementing<IModuleData>());

      return types;
    }

    private List<Type> FindConcreteTypesImplementing<TInterface>()
    {
      var result = new List<Type>();

      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        if (assembly.IsDynamic)
          continue;

        foreach (Type type in assembly.GetTypes())
        {
          if (typeof(TInterface).IsAssignableFrom(type) &&
              !type.IsAbstract &&
              !type.IsInterface)
          {
            result.Add(type);
          }
        }
      }

      return result;
    }

    private HashSet<string> BuildTrustedAssemblies(IEnumerable<Type> allowedTypes = null)
    {
      HashSet<string> trusted = new HashSet<string>(StringComparer.Ordinal);

      if (allowedTypes != null)
        foreach (Type type in allowedTypes)
          trusted.Add(type.Assembly.GetName().Name);

      string[] prefixes =
      {
        "System",
        "mscorlib",
        "netstandard",
        "UnityEngine",
        "Assembly-CSharp"
      };

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        string name = assembly.GetName().Name;
        if (prefixes.Any(p => name.StartsWith(p, StringComparison.Ordinal)))
          trusted.Add(name);
      }

      return trusted;
    }
  }
}