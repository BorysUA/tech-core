using System;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Data.Progress.Meta
{
  [Serializable]
  public class SessionInfo
  {
    public string ColonyName;
    public float Playtime;
    public int Seed;
    public GameDifficulty Difficulty;

    public SessionInfo(string colonyName, GameDifficulty difficulty, int seed)
    {
      ColonyName = colonyName;
      Difficulty = difficulty;
      Seed = seed;
    }
  }
}