using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.CodeBase.Infrastructure.Services.SaveService
{
  public class JsonSaveStorageService : ISaveStorageService
  {
    private const string JsonExtension = ".json";
    private const string BakExtension = ".bak";
    private const string TmpExtension = ".tmp";
    private const string MetaExtension = ".meta";

    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly string _saveRootPath = Application.persistentDataPath;
    private readonly string _applicationVersion = Application.version;
    private readonly ILogService _logService;

    private readonly Dictionary<SaveSlot, WeakReference<GameStateData>> _cache = new();

    private readonly JsonSerializerSettings _settings;

    public JsonSaveStorageService(ILogService logService)
    {
      _logService = logService;

      _settings = new JsonSerializerSettings
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
    }

    public async UniTask SaveGameAsync(GameStateData data, SaveSlot saveSlot, CancellationToken token = default)
    {
      _cache.Remove(saveSlot);
      string baseName = GetFileKey(saveSlot);

      string saveFilePath = Path.Combine(_saveRootPath, baseName + JsonExtension);
      string backupFilePath = Path.Combine(_saveRootPath, baseName + BakExtension);
      string tempFilePath = Path.Combine(_saveRootPath, baseName + TmpExtension);
      string metaFilePath = Path.Combine(_saveRootPath, baseName + MetaExtension);

      string tempBackupFilePath = backupFilePath + TmpExtension;
      string tempMetaFilePath = metaFilePath + TmpExtension;

      Directory.CreateDirectory(_saveRootPath);

      string json = JsonConvert.SerializeObject(data, _settings);

      await _semaphore.WaitAsync(token)
        .WaitAsync(TimeSpan.FromSeconds(30), TimeProvider.System, cancellationToken: token);

      try
      {
        try
        {
          await SaveProgress(json, tempFilePath, backupFilePath, tempBackupFilePath, saveFilePath, token);
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
        _semaphore.Release();

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
      if (_cache.TryGetValue(saveSlot, out WeakReference<GameStateData> weakReference))
        if (weakReference.TryGetTarget(out GameStateData gameStateData))
          return new LoadResult(LoadStatus.Success, gameStateData);

      string baseName = GetFileKey(saveSlot);

      string saveFilePath = Path.Combine(_saveRootPath, baseName + JsonExtension);

      try
      {
        if (File.Exists(saveFilePath))
        {
          string json = await File.ReadAllTextAsync(saveFilePath, token)
            .AsUniTask()
            .ContinueOnMainThread();
          
          token.ThrowIfCancellationRequested();

          GameStateData gameStateData = JsonConvert.DeserializeObject<GameStateData>(json, _settings);
          _cache.Add(saveSlot, new WeakReference<GameStateData>(gameStateData));
          return new LoadResult(LoadStatus.Success, gameStateData);
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
    }

    public async UniTask<IEnumerable<SaveMetaData>> GetAllSavesMeta()
    {
      List<SaveMetaData> metaDataList = new();

      foreach (string filePath in Directory.EnumerateFiles(_saveRootPath, "*.meta"))
      {
        try
        {
          string json = await File.ReadAllTextAsync(filePath)
            .AsUniTask()
            .ContinueOnMainThread();

          SaveMetaData meta = JsonConvert.DeserializeObject<SaveMetaData>(json);

          if (meta != null)
            metaDataList.Add(meta);
          else
            _logService.LogWarning(GetType(), $"Deserialization returned null for meta file: {filePath}");
        }
        catch (Exception ex)
        {
          _logService.LogWarning(GetType(), $"Failed to read or parse meta file: {filePath}. Error: {ex.Message}");
        }
      }

      return metaDataList
        .OrderByDescending(meta => meta.LastModifiedUtc)
        .ToList();
    }

    public void ClearSlotManual(SaveSlot saveSlot)
    {
      string baseName = GetFileKey(saveSlot);

      foreach (string ext in new[] { JsonExtension, BakExtension, MetaExtension })
      {
        string path = Path.Combine(_saveRootPath, baseName + ext);
        try
        {
          File.Delete(path);
        }
        catch (Exception exception)
        {
          _logService.LogError(GetType(), $"Failed to delete player progress in save slot [{saveSlot}]", exception);
        }
      }
    }

    public void Dispose()
    {
      _semaphore?.Dispose();
    }

    private async UniTask SaveProgress(string json, string tempFilePath, string backupFilePath,
      string tempBackupFilePath, string saveFilePath, CancellationToken token)
    {
      await UniTask.RunOnThreadPool(() =>
        {
          File.WriteAllText(tempFilePath, json);

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
        LastModifiedUtc = DateTime.Now,
        BuildVersion = _applicationVersion
      };

      string metaJson = JsonConvert.SerializeObject(saveMetaData, _settings);

      await UniTask.RunOnThreadPool(() =>
        {
          File.WriteAllText(tempMetaFilePath, metaJson);
          ReplaceFile(tempMetaFilePath, metaFilePath);
        }, cancellationToken: token, configureAwait: false)
        .ContinueOnMainThread();

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

    private string GetFileKey(SaveSlot slot) => slot switch
    {
      SaveSlot.Auto => "auto",
      SaveSlot.Quick => "quick",
      SaveSlot.Manual1 => "manual1",
      SaveSlot.Manual2 => "manual2",
      SaveSlot.Manual3 => "manual3",
      SaveSlot.Manual4 => "manual4",
      SaveSlot.Manual5 => "manual5",
      SaveSlot.Manual6 => "manual6",
      SaveSlot.Manual7 => "manual7",
      SaveSlot.Manual8 => "manual8",
      SaveSlot.Manual9 => "manual9",
      SaveSlot.Manual10 => "manual10",
      _ => throw new ArgumentOutOfRangeException(nameof(slot))
    };

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