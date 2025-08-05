using System;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [Serializable]
  public class TradeConfig
  {
    public ResourceRange[] PurchaseResources;
    public ResourceKind PaymentResource;

    public float ResourceAmountPerLevelMultiplier;
    public int MinResourcesCount;
    public int MaxResourcesCount => PurchaseResources?.Length ?? 0;
    public float OfferCloseCountdown;
    public float NextOfferOpenCountdown;
  }
}