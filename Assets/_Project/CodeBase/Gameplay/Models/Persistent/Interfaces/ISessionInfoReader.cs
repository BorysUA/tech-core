using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface ISessionInfoReader
  {
    public ReadOnlyReactiveProperty<string> ColonyName { get; }
    public ReadOnlyReactiveProperty<float> TotalPlayTime { get; }
    public ReadOnlyReactiveProperty<GameDifficulty> Difficulty { get; }
    int Seed { get; }
  }
}