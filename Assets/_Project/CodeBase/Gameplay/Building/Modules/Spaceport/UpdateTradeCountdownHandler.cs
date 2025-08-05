using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services.Command;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
{
  public class UpdateTradeCountdownHandler : ICommandHandler<UpdateTradeCountdownCommand, Unit>
  {
    private readonly ModuleContextResolver _moduleContextResolver;

    public UpdateTradeCountdownHandler(ModuleContextResolver moduleContextResolver)
    {
      _moduleContextResolver = moduleContextResolver;
    }

    public Unit Execute(in UpdateTradeCountdownCommand command)
    {
      ModuleContext<TradeModuleConfig, TradeData> moduleContext =
        _moduleContextResolver.Resolve<TradeModuleConfig, TradeData>(command.BuildingId);

      moduleContext.Data.OfferCloseCountdown = command.OfferCloseCountdown;
      moduleContext.Data.NextOfferOpenCountdown = command.NextOfferOpenCountdown;

      return Unit.Default;
    }
  }
}