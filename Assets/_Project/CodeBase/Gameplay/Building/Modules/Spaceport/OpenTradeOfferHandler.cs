using System;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Utility;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
{
  public class OpenTradeOfferHandler : ICommandHandler<OpenTradeOfferCommand, Unit>
  {
    private readonly ModuleContextResolver _moduleContextResolver;
    private readonly IProgressWriter _progressWriter;

    public OpenTradeOfferHandler(ModuleContextResolver moduleContextResolver, IProgressWriter progressWriter)
    {
      _moduleContextResolver = moduleContextResolver;
      _progressWriter = progressWriter;
    }

    public Unit Execute(in OpenTradeOfferCommand command)
    {
      ModuleContext<TradeModuleConfig, TradeData> moduleContext =
        _moduleContextResolver.Resolve<TradeModuleConfig, TradeData>(command.BuildingId);

      if (moduleContext.Data.CurrentOffer != null && moduleContext.Data.OfferCloseCountdown > 0)
        return Unit.Default;

      moduleContext.Data.CurrentOffer =
        CreateTradeOffer(command.BuildingId, moduleContext.Config.TradeConfig, moduleContext.Data);
      moduleContext.Data.OfferCloseCountdown = moduleContext.Config.TradeConfig.OfferCloseCountdown;
      moduleContext.Data.NextOfferOpenCountdown = 0;

      return Unit.Default;
    }

    private TradeOfferData CreateTradeOffer(int buildingId, TradeConfig tradeConfig, TradeData tradeData)
    {
      int totalPrice = 0;
      ResourceRange[] resources = tradeConfig.PurchaseResources;

      int resourceCount = Math.Clamp(MathUtils.LerpInt(tradeConfig.MinResourcesCount, tradeConfig.MaxResourcesCount,
          Math.Clamp(tradeConfig.ResourceAmountPerLevelMultiplier * tradeData.CompletedTradesCount, 0, 1)), 0,
        resources.Length);

      ResourceAmountData[] purchaseResources = new ResourceAmountData[resourceCount];

      int[] randomResourceIndices = new int[resources.Length];
      for (int i = 0; i < randomResourceIndices.Length; i++)
        randomResourceIndices[i] = i;

      int seed = MathUtils.Hash32(_progressWriter.GameStateModel.SessionInfoWriter.Seed, buildingId,
        tradeData.CompletedTradesCount);

      Random random = new Random(seed);
      MathUtils.ShuffleFirstKIndices(randomResourceIndices, resourceCount, random);

      for (int i = 0; i < resourceCount; i++)
      {
        ResourceRange resource = resources[randomResourceIndices[i]];
        int amount = random.Next(resource.Amount.Min, resource.Amount.Max + 1);

        purchaseResources[i] = new ResourceAmountData(resource.Resource.Kind, amount);

        int multiplier = 900 + random.Next(0, 201);
        long priceForThisResource = (long)purchaseResources[i].Amount * resource.Resource.Price * multiplier / 1000L;
        totalPrice += (int)priceForThisResource;
      }

      ResourceAmountData payment = new ResourceAmountData(tradeConfig.PaymentResource, totalPrice);

      return new TradeOfferData(purchaseResources, payment);
    }
  }
}