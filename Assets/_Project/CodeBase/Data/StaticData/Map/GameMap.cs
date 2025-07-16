using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Map
{
  [CreateAssetMenu(fileName = "GameMap", menuName = "ScriptableObjects/GameMap")]
  public class GameMap : ScriptableObject
  {
    public MapEntityData[] Entities;
  }
}