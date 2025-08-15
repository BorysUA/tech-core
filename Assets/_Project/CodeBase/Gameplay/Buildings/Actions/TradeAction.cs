using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Buildings.Modules.Trade;
using _Project.CodeBase.Gameplay.UI.Windows.Trade;
using _Project.CodeBase.UI.Services;

namespace _Project.CodeBase.Gameplay.Buildings.Actions
{
  public class TradeAction : IBuildingAction
  {
    public ActionType Type => ActionType.Trade;

    private readonly IWindowsService _windowsService;
    private TradeModule _tradeModule;

    public TradeAction(IWindowsService windowsService)
    {
      _windowsService = windowsService;
    }

    public void Setup(TradeModule tradeModule)
    {
      _tradeModule = tradeModule;
    }

    public void Execute()
    {
      if (_tradeModule.IsModuleWorking.CurrentValue)
        _windowsService.OpenWindow<TradeWindow, TradeViewModel, TradeModule>(_tradeModule,
          token: _tradeModule.Lifetime);
    }
  }
}