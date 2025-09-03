using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Services.Factories;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Meteorite
{
  public class MeteoriteSpawner
  {
    private readonly IGameplayFactory _gameplayFactory;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly ICoroutineProvider _coroutineProvider;
    private readonly ILogService _logService;

    private WaitForSeconds _waitSpawnInterval;
    private MeteoriteSpawnerConfig _config;
    private float _multiplier;
    private Coroutine _spawnMeteoritesCoroutine;

    public MeteoriteSpawner(IGameplayFactory gameplayFactory, IStaticDataProvider staticDataProvider,
      ICoroutineProvider coroutineProvider, ILogService logService)
    {
      _gameplayFactory = gameplayFactory;
      _staticDataProvider = staticDataProvider;
      _coroutineProvider = coroutineProvider;
      _logService = logService;
    }

    public void Initialize(float multiplier)
    {
      _multiplier = multiplier;
      _config = _staticDataProvider.GetMeteoriteSpawnerConfig();
      _waitSpawnInterval = new WaitForSeconds(_config.SpawnInterval);
    }

    public void Start()
    {
      _spawnMeteoritesCoroutine = _coroutineProvider.ExecuteCoroutine(SpawnMeteorites());
    }

    public void Stop()
    {
      _coroutineProvider.TerminateCoroutine(_spawnMeteoritesCoroutine);
    }


    private IEnumerator SpawnMeteorites()
    {
      while (true)
      {
        yield return _waitSpawnInterval;
        SpawnMeteorite().Forget();
      }
    }

    private async UniTaskVoid SpawnMeteorite()
    {
      if (_config.PossibleMeteorites == null || _config.PossibleMeteorites.Count == 0)
      {
        _logService.LogWarning(GetType(), "No meteorites to spawn!");
        return;
      }

      int randomIndex = Random.Range(0, _config.PossibleMeteorites.Count);
      MeteoriteConfig meteoriteConfig = _config.PossibleMeteorites[randomIndex];

      var targetPosition = GetTargetPosition(meteoriteConfig.ExplosionArea);
      var spawnPosition = targetPosition + _config.SpawnPointOffset;

      MeteoriteViewModel meteoriteViewModel =
        await _gameplayFactory.CreateMeteorite(meteoriteConfig.Type, spawnPosition);

      meteoriteViewModel.Setup(targetPosition, spawnPosition, meteoriteConfig);
    }

    private Vector3 GetTargetPosition(Vector2Int explosionAreaSize)
    {
      Vector2Int randomCell = GridUtils.GetRandomCell();
      Vector3 worldPivot = GridUtils.GetWorldPivot(randomCell);

      if (explosionAreaSize.sqrMagnitude > Vector2Int.one.sqrMagnitude)
      {
        Vector3 snappedPosition = GridUtils.GetSnappedPosition(worldPivot, explosionAreaSize);
        List<Vector2Int> explosionAreaCells =
          GridUtils.GetCells(snappedPosition, explosionAreaSize);
        worldPivot = GridUtils.GetWorldPivot(explosionAreaCells);
      }

      return worldPivot;
    }
  }
}