using System;

namespace _Project.CodeBase.Data.Progress.Building.ModuleData
{
  [Serializable]
  public class TradeData : IReadOnlyTradeData
  {
    public int CompletedTradesCount { set; get; }
    public TradeOfferData CurrentOffer { get; set; }
    public float OfferCloseCountdown { get; set; }
    public float NextOfferOpenCountdown { get; set; }

    public TradeData(int completedTradesCount)
    {
      CompletedTradesCount = completedTradesCount;
    }
  }
}