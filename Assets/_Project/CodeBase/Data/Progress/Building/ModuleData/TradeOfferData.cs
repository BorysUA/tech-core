using System;
using _Project.CodeBase.Data.Progress.ResourceData;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class TradeOfferData
  {
    public ResourceAmountData[] ResourcesToSell { get; private set; }
    public ResourceAmountData Reward { get; private set; }

    public TradeOfferData(ResourceAmountData[] resourcesToSell, ResourceAmountData reward)
    {
      ResourcesToSell = resourcesToSell;
      Reward = reward;
    }
  }
}