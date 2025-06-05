using System;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Data.Progress.Meta
{
  [Serializable]
  public class SessionInfo
  {
    public string ColonyName;
    public float Playtime;
    public GameDifficulty Difficulty;

    public SessionInfo(string colonyName, GameDifficulty difficulty)
    {
      ColonyName = colonyName;
      Difficulty = difficulty;
    }
  }
}