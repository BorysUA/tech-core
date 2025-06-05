using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Menu.UI.DifficultySelection;
using R3;

namespace _Project.CodeBase.Gameplay.Data
{
  public class SessionInfoProxy
  {
    public readonly ReactiveProperty<float> SessionPlayTime = new(0);
    public ReactiveProperty<string> ColonyName { get; }
    public ReactiveProperty<float> TotalPlayTime { get; }
    public ReactiveProperty<GameDifficulty> Difficulty { get; }

    public SessionInfoProxy(SessionInfo sessionInfo)
    {
      ColonyName = new ReactiveProperty<string>(sessionInfo.ColonyName);
      Difficulty = new ReactiveProperty<GameDifficulty>(sessionInfo.Difficulty);
      TotalPlayTime = new ReactiveProperty<float>(sessionInfo.Playtime);

      TotalPlayTime.Subscribe(value => sessionInfo.Playtime = value);
      ColonyName.Subscribe(value => sessionInfo.ColonyName = value);
      Difficulty.Subscribe(value => sessionInfo.Difficulty = value);
    }
  }
}