using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Gameplay.Signals.Domain;
using Zenject;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
{
  public class FulfillTradeOfferHandler : ICommandHandler<FulfillTradeOfferCommand, bool>
  {
    private readonly ModuleContextResolver _moduleContextResolver;
    private readonly IResourceMutator _resourceMutator;

    public FulfillTradeOfferHandler(ModuleContextResolver moduleContextResolver, IResourceMutator resourceMutator)
    {
      _moduleContextResolver = moduleContextResolver;
      _resourceMutator = resourceMutator;
    }

    public bool Execute(in FulfillTradeOfferCommand command)
    {
      ModuleContext<TradeModuleConfig, TradeData> moduleContext =
        _moduleContextResolver.Resolve<TradeModuleConfig, TradeData>(command.BuildingId);

      Span<ResourceAmountData> toAdd = stackalloc ResourceAmountData[1] { moduleContext.Data.CurrentOffer.Reward };
      Span<ResourceAmountData> resultBuffer = stackalloc ResourceAmountData[1];

      ResourceMutationResult resourceMutationResult =
        _resourceMutator.TryTransferFlexible(moduleContext.Data.CurrentOffer.ResourcesToSell, toAdd, resultBuffer);

      if (resourceMutationResult.IsSuccess)
      {
        moduleContext.Data.CompletedTradesCount++;
        moduleContext.Data.CurrentOffer = null;
        moduleContext.Data.OfferCloseCountdown = 0;

        return true;
      }

      return false;
    }
  }
}