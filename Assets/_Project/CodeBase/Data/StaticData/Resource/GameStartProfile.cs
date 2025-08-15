using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Resource
{
  [CreateAssetMenu(fileName = "GameStartProfile", menuName = "ScriptableObjects/GameStartProfile",
    order = 0)]
  public class GameStartProfile : ScriptableObject
  {
    public GameDifficulty Difficulty;
    public List<GameResourceData> Resources;
  }
}