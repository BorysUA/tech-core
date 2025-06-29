using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Services.RemoteConfigsService;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Meteorite
{
  [CreateAssetMenu(fileName = "MeteoriteSpawnerConfig", menuName = "ScriptableObjects/MeteoriteSpawnerConfig")]
  public class MeteoriteSpawnerConfig : ScriptableObject
  {
    [RemoteKey(RemoteConfigKeys.MeteoriteSpawnInterval)]
    public float SpawnInterval = 30f;
    public int SpawnCount = 1;
    public Vector3 SpawnPointOffset = new(5, 10, -5);
    public List<MeteoriteConfig> PossibleMeteorites = new();
  }
}