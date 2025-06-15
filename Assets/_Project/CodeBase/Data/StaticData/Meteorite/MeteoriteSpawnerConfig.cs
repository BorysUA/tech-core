using System.Collections.Generic;
using _Project.CodeBase.Services.RemoteConfigsService;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Meteorite
{
  [CreateAssetMenu(fileName = "MeteoriteSpawnerConfig", menuName = "ScriptableObjects/MeteoriteSpawnerConfig")]
  public class MeteoriteSpawnerConfig : ScriptableObject
  {
    [RemoteKey("meteorites_spawn_interval")]
    public float SpawnInterval = 30f;
    public int SpawnCount = 3;
    public Vector3 SpawnPointOffset = new(5, 10, -5);
    public List<MeteoriteConfig> PossibleMeteorites = new();
  }
}