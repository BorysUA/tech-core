using System.Collections;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Meteorite;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Meteorite
{
  public class MeteoriteSpawner
  {
    private readonly IGameplayFactory _gameplayFactory;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly WaitForSeconds _waitSpawnInterval;
    private readonly IGridService _gridService;
    private readonly ILogService _logService;

    private readonly MeteoriteSpawnerConfig _config;
    private Coroutine _spawnMeteoritesCoroutine;

    public MeteoriteSpawner(IGameplayFactory gameplayFactory, IStaticDataProvider staticDataProvider,
      ICoroutineRunner coroutineRunner, IGridService gridService, ILogService logService)
    {
      _gameplayFactory = gameplayFactory;
      _coroutineRunner = coroutineRunner;
      _gridService = gridService;
      _logService = logService;

      _config = staticDataProvider.GetMeteoriteSpawnerConfig();
      _waitSpawnInterval = new WaitForSeconds(_config.SpawnInterval);
    }

    public void Start()
    {
      _spawnMeteoritesCoroutine = _coroutineRunner.ExecuteCoroutine(SpawnMeteorites());
    }

    public void Stop()
    {
      _coroutineRunner.TerminateCoroutine(_spawnMeteoritesCoroutine);
    }

    private IEnumerator SpawnMeteorites()
    {
      while (true)
      {
        yield return _waitSpawnInterval;
        SpawnMeteorite();
      }
    }

    private async void SpawnMeteorite()
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
      Vector2Int randomCell = _gridService.GetRandomCell();
      Vector3 worldPivot = _gridService.GetWorldPivot(randomCell);

      if (explosionAreaSize.sqrMagnitude > Vector2Int.one.sqrMagnitude)
      {
        Vector3 snappedPosition = _gridService.GetSnappedPosition(worldPivot, explosionAreaSize);
        List<Vector2Int> explosionAreaCells =
          _gridService.GetCells(snappedPosition, explosionAreaSize);
        worldPivot = _gridService.GetWorldPivot(explosionAreaCells);
      }

      return worldPivot;
    }
  }
}