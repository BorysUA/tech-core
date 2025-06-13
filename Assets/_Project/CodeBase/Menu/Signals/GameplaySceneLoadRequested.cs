using _Project.CodeBase.Data.Settings;

namespace _Project.CodeBase.Menu.Signals
{
  public class GameplaySceneLoadRequested
  {
    public GameplaySettings GameplaySettings { get; private set; }

    public GameplaySceneLoadRequested(GameplaySettings gameplaySettings)
    {
      GameplaySettings = gameplaySettings;
    }
  }
}