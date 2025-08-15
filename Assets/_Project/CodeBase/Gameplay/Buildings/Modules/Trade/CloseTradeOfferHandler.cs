using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Trade
{
  public class CloseTradeOfferHandler : ICommandHandler<CloseTradeOfferCommand, Unit>
  {
    private readonly ModuleContextResolver _moduleContextResolver;

    public CloseTradeOfferHandler(ModuleContextResolver moduleContextResolver)
    {
      _moduleContextResolver = moduleContextResolver;
    }

    public Unit Execute(in CloseTradeOfferCommand command)
    {
      ModuleContext<TradeModuleConfig, TradeData> moduleContext =
        _moduleContextResolver.Resolve<TradeModuleConfig, TradeData>(command.BuildingId);

      moduleContext.Data.CurrentOffer = null;
      moduleContext.Data.OfferCloseCountdown = 0;
      moduleContext.Data.NextOfferOpenCountdown = moduleContext.Config.TradeConfig.NextOfferOpenCountdown;
      return Unit.Default;
    }
  }
}