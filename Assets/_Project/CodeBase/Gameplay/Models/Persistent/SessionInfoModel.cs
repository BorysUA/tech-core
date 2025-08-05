using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Gameplay.Models.Persistent
{
  public class SessionInfoModel : ISessionInfoWriter
  {
    public ReactiveProperty<string> ColonyName { get; }
    public ReactiveProperty<float> TotalPlayTime { get; }
    public int Seed { get; }
    public ReactiveProperty<GameDifficulty> Difficulty { get; }

    ReadOnlyReactiveProperty<string> ISessionInfoReader.ColonyName => ColonyName;
    ReadOnlyReactiveProperty<float> ISessionInfoReader.TotalPlayTime => TotalPlayTime;
    ReadOnlyReactiveProperty<GameDifficulty> ISessionInfoReader.Difficulty => Difficulty;

    public SessionInfoModel(SessionInfo sessionInfo)
    {
      ColonyName = new ReactiveProperty<string>(sessionInfo.ColonyName);
      Difficulty = new ReactiveProperty<GameDifficulty>(sessionInfo.Difficulty);
      TotalPlayTime = new ReactiveProperty<float>(sessionInfo.Playtime);
      Seed = sessionInfo.Seed;

      TotalPlayTime.Subscribe(value => sessionInfo.Playtime = value);
      ColonyName.Subscribe(value => sessionInfo.ColonyName = value);
      Difficulty.Subscribe(value => sessionInfo.Difficulty = value);
    }
  }
}