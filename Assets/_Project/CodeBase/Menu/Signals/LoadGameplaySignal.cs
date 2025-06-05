using _Project.CodeBase.Data;
using _Project.CodeBase.Data.Settings;

namespace _Project.CodeBase.Menu.Signals
{
  public class LoadGameplaySignal
  {
    public GameplaySettings GameplaySettings { get; private set; }

    public LoadGameplaySignal(GameplaySettings gameplaySettings)
    {
      GameplaySettings = gameplaySettings;
    }
  }
}