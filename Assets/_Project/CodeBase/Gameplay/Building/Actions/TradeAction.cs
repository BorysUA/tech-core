using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Modules.Spaceport;
using _Project.CodeBase.Gameplay.UI.Windows.Trade;
using _Project.CodeBase.UI.Services;

namespace _Project.CodeBase.Gameplay.Building.Actions
{
  public class TradeAction : IBuildingAction
  {
    public ActionType Type => ActionType.Trade;

    private readonly IWindowsService _windowsService;
    private SpaceportTradeModule _tradeModule;

    public TradeAction(IWindowsService windowsService)
    {
      _windowsService = windowsService;
    }

    public void Setup(SpaceportTradeModule spaceportTradeModule)
    {
      _tradeModule = spaceportTradeModule;
    }

    public void Execute()
    {
      if (_tradeModule.IsModuleWorking.CurrentValue)
      {
        _windowsService.OpenWindow<TradeWindow, TradeViewModel, SpaceportTradeModule>(_tradeModule,
          token: _tradeModule.Lifetime);
      }
    }
  }
}