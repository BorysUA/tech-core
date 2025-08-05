using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent.Interfaces
{
  public interface ISessionInfoWriter : ISessionInfoReader
  {
    public new ReactiveProperty<string> ColonyName { get; }
    public new ReactiveProperty<float> TotalPlayTime { get; }
    public new ReactiveProperty<GameDifficulty> Difficulty { get; }
  }
}