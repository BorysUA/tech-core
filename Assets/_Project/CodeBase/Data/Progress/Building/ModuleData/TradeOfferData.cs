using System;
using _Project.CodeBase.Data.Progress.ResourceData;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class TradeOfferData
  {
    public ResourceAmountData[] PurchaseResources { get; private set; }
    public ResourceAmountData Payment { get; private set; }

    public TradeOfferData(ResourceAmountData[] purchaseResources, ResourceAmountData payment)
    {
      PurchaseResources = purchaseResources;
      Payment = payment;
    }
  }
}